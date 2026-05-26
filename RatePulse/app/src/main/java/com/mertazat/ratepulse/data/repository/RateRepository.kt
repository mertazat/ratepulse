package com.mertazat.ratepulse.data.repository

import com.mertazat.ratepulse.data.model.LatestRatesResponse
import com.mertazat.ratepulse.data.model.RateHistoryResponse
import com.mertazat.ratepulse.data.remote.ApiService
import javax.inject.Inject

class RateRepository @Inject constructor(
    private val apiService: ApiService
) {
    suspend fun getLatestRates(): Result<LatestRatesResponse> = runCatching {
        val response = apiService.getLatestRates()
        if (response.isSuccessful) {
            response.body()?.data ?: error("No data")
        } else {
            error("API error: ${response.code()}")
        }
    }

    suspend fun getRateHistory(
        from: String = "USD",
        to: String = "TRY",
        limit: Int = 24
    ): Result<RateHistoryResponse> = runCatching {
        val response = apiService.getRateHistory(from, to, limit)
        if (response.isSuccessful) {
            response.body()?.data ?: error("No data")
        } else {
            error("API error: ${response.code()}")
        }
    }
}
