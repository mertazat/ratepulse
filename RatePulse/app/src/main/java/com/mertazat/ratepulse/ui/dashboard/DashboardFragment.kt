package com.mertazat.ratepulse.ui.dashboard

import android.graphics.Color
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ArrayAdapter
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import com.github.mikephil.charting.components.XAxis
import com.github.mikephil.charting.data.Entry
import com.github.mikephil.charting.data.LineData
import com.github.mikephil.charting.data.LineDataSet
import com.github.mikephil.charting.formatter.IndexAxisValueFormatter
import com.mertazat.ratepulse.RatePulseApp
import com.mertazat.ratepulse.databinding.FragmentDashboardBinding

class DashboardFragment : Fragment() {

    private var _binding: FragmentDashboardBinding? = null
    private val binding get() = _binding!!
    private lateinit var viewModel: DashboardViewModel

    private val currencies = listOf("USD", "EUR", "TRY", "GBP", "JPY", "AUD", "CAD", "CHF", "CNY")

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?
    ): View {
        _binding = FragmentDashboardBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        val appContainer = (requireActivity().application as RatePulseApp).container
        viewModel = ViewModelProvider(
            this,
            DashboardViewModel.Factory(appContainer.rateRepository)
        )[DashboardViewModel::class.java]

        setupCurrencySpinners()
        setupChart()
        observeData()

        binding.btnRefresh.setOnClickListener {
            viewModel.loadRates()
            viewModel.loadHistory(
                binding.spinnerFrom.selectedItem.toString(),
                binding.spinnerTo.selectedItem.toString()
            )
        }

        binding.btnApplyPair.setOnClickListener {
            viewModel.loadHistory(
                binding.spinnerFrom.selectedItem.toString(),
                binding.spinnerTo.selectedItem.toString()
            )
        }
    }

    private fun setupCurrencySpinners() {
        val adapter = ArrayAdapter(requireContext(), android.R.layout.simple_spinner_item, currencies)
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
        binding.spinnerFrom.adapter = adapter
        binding.spinnerTo.adapter = adapter
        // USD/TRY varsayılan
        binding.spinnerFrom.setSelection(0)
        binding.spinnerTo.setSelection(2)
    }

    private fun setupChart() {
        binding.lineChart.apply {
            description.isEnabled = false
            setTouchEnabled(true)
            isDragEnabled = true
            setScaleEnabled(true)
            setPinchZoom(true)
            setDrawGridBackground(false)
            legend.isEnabled = false

            xAxis.apply {
                position = XAxis.XAxisPosition.BOTTOM
                granularity = 1f
                setDrawGridLines(false)
                textColor = Color.GRAY
                textSize = 10f
            }
            axisLeft.apply {
                setDrawGridLines(true)
                gridColor = Color.parseColor("#EEEEEE")
                textColor = Color.GRAY
            }
            axisRight.isEnabled = false
        }
    }

    private fun observeData() {
        viewModel.isLoading.observe(viewLifecycleOwner) { loading ->
            binding.progressBar.visibility = if (loading) View.VISIBLE else View.GONE
        }

        viewModel.rates.observe(viewLifecycleOwner) { ratesData ->
            ratesData ?: return@observe
            binding.tvFetchTime.text = "Son güncelleme: ${ratesData.fetchedAt}"

            // Önemli çiftleri göster
            val mainPairs = listOf(
                "TRY" to "USD/TRY",
                "EUR" to "USD/EUR",
                "GBP" to "USD/GBP",
                "JPY" to "USD/JPY"
            )

            val rateViews = listOf(
                binding.cardRate1 to mainPairs[0],
                binding.cardRate2 to mainPairs[1],
                binding.cardRate3 to mainPairs[2],
                binding.cardRate4 to mainPairs[3]
            )

            rateViews.forEach { (card, pair) ->
                val rate = ratesData.rates[pair.first]
                card.tvPairName.text = pair.second
                card.tvRateValue.text = rate?.let { "%.4f".format(it) } ?: "—"
            }
        }

        viewModel.history.observe(viewLifecycleOwner) { historyData ->
            historyData ?: return@observe
            updateChart(historyData)
        }

        viewModel.error.observe(viewLifecycleOwner) { error ->
            error ?: return@observe
            binding.tvFetchTime.text = "Hata: $error"
        }
    }

    private fun updateChart(historyData: com.mertazat.ratepulse.data.model.RateHistoryResponse) {
        val entries = historyData.history.mapIndexed { index, point ->
            Entry(index.toFloat(), point.rate.toFloat())
        }

        val labels = historyData.history.map { point ->
            point.timestamp.take(16).replace("T", " ")
        }

        val dataSet = LineDataSet(entries, "${historyData.fromCurrency}/${historyData.toCurrency}").apply {
            color = Color.parseColor("#6366F1")
            valueTextColor = Color.TRANSPARENT
            lineWidth = 2f
            circleRadius = 2f
            setCircleColor(Color.parseColor("#6366F1"))
            setDrawFilled(true)
            fillColor = Color.parseColor("#6366F1")
            fillAlpha = 30
            mode = LineDataSet.Mode.CUBIC_BEZIER
        }

        binding.lineChart.apply {
            xAxis.valueFormatter = IndexAxisValueFormatter(labels)
            xAxis.labelCount = minOf(labels.size, 6)
            data = LineData(dataSet)
            animateX(500)
            invalidate()
        }

        binding.tvChartTitle.text = "${historyData.fromCurrency}/${historyData.toCurrency} — Son 24 Kayıt"
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
