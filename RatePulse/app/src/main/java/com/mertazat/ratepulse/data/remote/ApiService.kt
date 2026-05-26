package com.mertazat.ratepulse.data.remote

import com.mertazat.ratepulse.data.model.*
import retrofit2.Response
import retrofit2.http.*

interface ApiService {

    // Auth
    @POST("api/auth/register")
    suspend fun register(@Body request: RegisterRequest): Response<SimpleApiResponse>

    // Rates
    @GET("api/rates/latest")
    suspend fun getLatestRates(): Response<ApiResponse<LatestRatesResponse>>

    @GET("api/rates/history")
    suspend fun getRateHistory(
        @Query("from") from: String = "USD",
        @Query("to") to: String = "TRY",
        @Query("limit") limit: Int = 24
    ): Response<ApiResponse<RateHistoryResponse>>

    // Alerts
    @GET("api/alerts")
    suspend fun getAlerts(): Response<ApiResponse<List<RateAlert>>>

    @POST("api/alerts")
    suspend fun createAlert(@Body request: CreateAlertRequest): Response<SimpleApiResponse>

    @DELETE("api/alerts/{id}")
    suspend fun deleteAlert(@Path("id") id: String): Response<SimpleApiResponse>

    // User
    @GET("api/user/profile")
    suspend fun getProfile(): Response<ApiResponse<UserProfile>>

    @PUT("api/user/profile")
    suspend fun updateProfile(@Body request: UpdateProfileRequest): Response<SimpleApiResponse>

    @PUT("api/user/fcm-token")
    suspend fun updateFcmToken(@Body request: UpdateFcmTokenRequest): Response<SimpleApiResponse>
}
