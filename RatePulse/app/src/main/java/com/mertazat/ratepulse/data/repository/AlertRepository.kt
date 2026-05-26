package com.mertazat.ratepulse.data.repository

import com.mertazat.ratepulse.data.model.CreateAlertRequest
import com.mertazat.ratepulse.data.model.RateAlert
import com.mertazat.ratepulse.data.remote.ApiService
import javax.inject.Inject

class AlertRepository @Inject constructor(
    private val apiService: ApiService
) {
    suspend fun getAlerts(): Result<List<RateAlert>> = runCatching {
        val response = apiService.getAlerts()
        if (response.isSuccessful) {
            response.body()?.data ?: emptyList()
        } else {
            error("API error: ${response.code()}")
        }
    }

    suspend fun createAlert(request: CreateAlertRequest): Result<String> = runCatching {
        val response = apiService.createAlert(request)
        if (response.isSuccessful) {
            response.body()?.message ?: "Alarm oluşturuldu"
        } else {
            error("API error: ${response.code()}")
        }
    }

    suspend fun deleteAlert(id: String): Result<String> = runCatching {
        val response = apiService.deleteAlert(id)
        if (response.isSuccessful) {
            response.body()?.message ?: "Alarm silindi"
        } else {
            error("API error: ${response.code()}")
        }
    }
}
