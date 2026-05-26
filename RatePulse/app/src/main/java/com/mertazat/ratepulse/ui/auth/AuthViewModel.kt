package com.mertazat.ratepulse.ui.auth

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewModelScope
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.messaging.FirebaseMessaging
import com.mertazat.ratepulse.data.model.RegisterRequest
import com.mertazat.ratepulse.data.repository.UserRepository
import kotlinx.coroutines.launch
import kotlinx.coroutines.tasks.await

class AuthViewModel(private val userRepository: UserRepository) : ViewModel() {

    private val auth = FirebaseAuth.getInstance()

    private val _authState = MutableLiveData<AuthState>()
    val authState: LiveData<AuthState> = _authState

    fun login(email: String, password: String) {
        _authState.value = AuthState.Loading
        viewModelScope.launch {
            try {
                auth.signInWithEmailAndPassword(email, password).await()
                val fcmToken = getFcmToken()
                if (fcmToken != null) userRepository.updateFcmToken(fcmToken)
                _authState.value = AuthState.Success
            } catch (e: Exception) {
                _authState.value = AuthState.Error(e.message ?: "Giriş başarısız")
            }
        }
    }

    fun register(email: String, password: String, displayName: String) {
        _authState.value = AuthState.Loading
        viewModelScope.launch {
            try {
                val result = auth.createUserWithEmailAndPassword(email, password).await()
                val uid = result.user?.uid ?: error("UID alınamadı")
                val fcmToken = getFcmToken() ?: ""
                val registerResult = userRepository.register(RegisterRequest(uid, email, displayName, fcmToken))
                registerResult.fold(
                    onSuccess = { _authState.value = AuthState.Success },
                    onFailure = { _authState.value = AuthState.Error(it.message ?: "Kayıt başarısız") }
                )
            } catch (e: Exception) {
                _authState.value = AuthState.Error(e.message ?: "Kayıt başarısız")
            }
        }
    }

    fun isLoggedIn() = auth.currentUser != null

    private suspend fun getFcmToken(): String? = try {
        FirebaseMessaging.getInstance().token.await()
    } catch (e: Exception) { null }

    class Factory(private val userRepository: UserRepository) : ViewModelProvider.Factory {
        override fun <T : ViewModel> create(modelClass: Class<T>): T {
            @Suppress("UNCHECKED_CAST")
            return AuthViewModel(userRepository) as T
        }
    }
}

sealed class AuthState {
    object Loading : AuthState()
    object Success : AuthState()
    object LoggedOut : AuthState()
    data class Error(val message: String) : AuthState()
}
