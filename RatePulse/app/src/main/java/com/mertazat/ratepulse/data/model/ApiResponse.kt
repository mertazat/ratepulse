package com.mertazat.ratepulse.data.model

data class ApiResponse<T>(
    val success: Boolean,
    val message: String?,
    val data: T?
)

data class SimpleApiResponse(
    val success: Boolean,
    val message: String?
)
