package com.mertazat.ratepulse.data.model

data class UserProfile(
    val uid: String,
    val email: String,
    val displayName: String,
    val preferredCurrencies: List<String>,
    val role: String,
    val createdAt: String
)

data class RegisterRequest(
    val uid: String,
    val email: String,
    val displayName: String,
    val fcmToken: String
)

data class UpdateProfileRequest(
    val displayName: String,
    val preferredCurrencies: List<String>
)

data class UpdateFcmTokenRequest(
    val fcmToken: String
)
