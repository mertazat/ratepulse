package com.mertazat.ratepulse.ui.dashboard

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewModelScope
import com.mertazat.ratepulse.data.model.LatestRatesResponse
import com.mertazat.ratepulse.data.model.RateHistoryResponse
import com.mertazat.ratepulse.data.repository.RateRepository
import kotlinx.coroutines.launch

class DashboardViewModel(private val rateRepository: RateRepository) : ViewModel() {

    private val _rates = MutableLiveData<LatestRatesResponse?>()
    val rates: LiveData<LatestRatesResponse?> = _rates

    private val _history = MutableLiveData<RateHistoryResponse?>()
    val history: LiveData<RateHistoryResponse?> = _history

    private val _isLoading = MutableLiveData(false)
    val isLoading: LiveData<Boolean> = _isLoading

    private val _error = MutableLiveData<String?>()
    val error: LiveData<String?> = _error

    var selectedFrom = "USD"
    var selectedTo = "TRY"

    fun loadRates() {
        viewModelScope.launch {
            _isLoading.value = true
            rateRepository.getLatestRates().fold(
                onSuccess = { _rates.value = it },
                onFailure = { _error.value = it.message }
            )
            _isLoading.value = false
        }
    }

    fun loadHistory(from: String = selectedFrom, to: String = selectedTo) {
        selectedFrom = from
        selectedTo = to
        viewModelScope.launch {
            rateRepository.getRateHistory(from, to).fold(
                onSuccess = { _history.value = it },
                onFailure = { _error.value = it.message }
            )
        }
    }

    init {
        loadRates()
        loadHistory()
    }

    class Factory(private val rateRepository: RateRepository) : ViewModelProvider.Factory {
        override fun <T : ViewModel> create(modelClass: Class<T>): T {
            @Suppress("UNCHECKED_CAST")
            return DashboardViewModel(rateRepository) as T
        }
    }
}
