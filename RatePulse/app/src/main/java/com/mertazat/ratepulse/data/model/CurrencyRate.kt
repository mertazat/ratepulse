package com.mertazat.ratepulse.data.model

data class LatestRatesResponse(
    val base: String,
    val fetchedAt: String,
    val rates: Map<String, Double>
)

data class RateHistoryResponse(
    val fromCurrency: String,
    val toCurrency: String,
    val history: List<RateHistoryPoint>
)

data class RateHistoryPoint(
    val timestamp: String,
    val rate: Double
)
