package com.mertazat.ratepulse.data.model

data class RateAlert(
    val id: String,
    val fromCurrency: String,
    val toCurrency: String,
    val targetRate: Double,
    val direction: String, // "above" | "below"
    val isActive: Boolean,
    val triggeredAt: String?,
    val createdAt: String
)

data class CreateAlertRequest(
    val fromCurrency: String,
    val toCurrency: String,
    val targetRate: Double,
    val direction: String
)
