package com.mertazat.ratepulse.ui.alerts

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewModelScope
import com.mertazat.ratepulse.data.model.CreateAlertRequest
import com.mertazat.ratepulse.data.model.RateAlert
import com.mertazat.ratepulse.data.repository.AlertRepository
import kotlinx.coroutines.launch

class AlertsViewModel(private val alertRepository: AlertRepository) : ViewModel() {

    private val _alerts = MutableLiveData<List<RateAlert>>()
    val alerts: LiveData<List<RateAlert>> = _alerts

    private val _isLoading = MutableLiveData(false)
    val isLoading: LiveData<Boolean> = _isLoading

    private val _message = MutableLiveData<String?>()
    val message: LiveData<String?> = _message

    fun loadAlerts() {
        viewModelScope.launch {
            _isLoading.value = true
            alertRepository.getAlerts().fold(
                onSuccess = { _alerts.value = it },
                onFailure = { _message.value = it.message }
            )
            _isLoading.value = false
        }
    }

    fun createAlert(from: String, to: String, targetRate: Double, direction: String) {
        viewModelScope.launch {
            _isLoading.value = true
            alertRepository.createAlert(CreateAlertRequest(from, to, targetRate, direction)).fold(
                onSuccess = { _message.value = "Alarm oluşturuldu"; loadAlerts() },
                onFailure = { _message.value = it.message }
            )
            _isLoading.value = false
        }
    }

    fun deleteAlert(id: String) {
        viewModelScope.launch {
            alertRepository.deleteAlert(id).fold(
                onSuccess = { _message.value = "Alarm silindi"; loadAlerts() },
                onFailure = { _message.value = it.message }
            )
        }
    }

    init { loadAlerts() }

    class Factory(private val alertRepository: AlertRepository) : ViewModelProvider.Factory {
        override fun <T : ViewModel> create(modelClass: Class<T>): T {
            @Suppress("UNCHECKED_CAST")
            return AlertsViewModel(alertRepository) as T
        }
    }
}
