package com.mertazat.ratepulse.data.repository

import com.mertazat.ratepulse.data.model.RegisterRequest
import com.mertazat.ratepulse.data.model.UpdateFcmTokenRequest
import com.mertazat.ratepulse.data.model.UpdateProfileRequest
import com.mertazat.ratepulse.data.model.UserProfile
import com.mertazat.ratepulse.data.remote.ApiService

class UserRepository(private val apiService: ApiService) {

    suspend fun register(request: RegisterRequest): Result<String> = runCatching {
        val response = apiService.register(request)
        if (response.isSuccessful) {
            response.body()?.message ?: "OK"
        } else {
            error("API error: ${response.code()}")
        }
    }

    suspend fun getProfile(): Result<UserProfile> = runCatching {
        val response = apiService.getProfile()
        if (response.isSuccessful) {
            response.body()?.data ?: error("No data")
        } else {
            error("API error: ${response.code()}")
        }
    }

    suspend fun updateProfile(request: UpdateProfileRequest): Result<String> = runCatching {
        val response = apiService.updateProfile(request)
        if (response.isSuccessful) {
            response.body()?.message ?: "OK"
        } else {
            error("API error: ${response.code()}")
        }
    }

    suspend fun updateFcmToken(token: String): Result<String> = runCatching {
        val response = apiService.updateFcmToken(UpdateFcmTokenRequest(token))
        if (response.isSuccessful) {
            response.body()?.message ?: "OK"
        } else {
            error("API error: ${response.code()}")
        }
    }
}
