package com.mertazat.ratepulse.ui.profile

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewModelScope
import com.mertazat.ratepulse.data.model.UpdateProfileRequest
import com.mertazat.ratepulse.data.model.UserProfile
import com.mertazat.ratepulse.data.repository.UserRepository
import kotlinx.coroutines.launch

class ProfileViewModel(private val userRepository: UserRepository) : ViewModel() {

    private val _profile = MutableLiveData<UserProfile?>()
    val profile: LiveData<UserProfile?> = _profile

    private val _isLoading = MutableLiveData(false)
    val isLoading: LiveData<Boolean> = _isLoading

    private val _message = MutableLiveData<String?>()
    val message: LiveData<String?> = _message

    fun loadProfile() {
        viewModelScope.launch {
            _isLoading.value = true
            userRepository.getProfile().fold(
                onSuccess = { _profile.value = it },
                onFailure = { _message.value = it.message }
            )
            _isLoading.value = false
        }
    }

    fun updateProfile(displayName: String, preferredCurrencies: List<String>) {
        viewModelScope.launch {
            _isLoading.value = true
            userRepository.updateProfile(UpdateProfileRequest(displayName, preferredCurrencies)).fold(
                onSuccess = {
                    _message.value = "Profil güncellendi"
                    loadProfile()
                },
                onFailure = { _message.value = it.message }
            )
            _isLoading.value = false
        }
    }

    init {
        loadProfile()
    }

    class Factory(private val userRepository: UserRepository) : ViewModelProvider.Factory {
        override fun <T : ViewModel> create(modelClass: Class<T>): T {
            @Suppress("UNCHECKED_CAST")
            return ProfileViewModel(userRepository) as T
        }
    }
}
