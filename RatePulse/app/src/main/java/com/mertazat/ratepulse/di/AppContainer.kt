package com.mertazat.ratepulse.di

import com.mertazat.ratepulse.data.remote.ApiClient
import com.mertazat.ratepulse.data.remote.ApiService
import com.mertazat.ratepulse.data.repository.AlertRepository
import com.mertazat.ratepulse.data.repository.RateRepository
import com.mertazat.ratepulse.data.repository.UserRepository

class AppContainer {
    val apiService: ApiService = ApiClient.create()
    val rateRepository = RateRepository(apiService)
    val alertRepository = AlertRepository(apiService)
    val userRepository = UserRepository(apiService)
}
