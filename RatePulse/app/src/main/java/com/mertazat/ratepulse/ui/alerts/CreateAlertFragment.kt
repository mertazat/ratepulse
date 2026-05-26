package com.mertazat.ratepulse.ui.alerts

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ArrayAdapter
import android.widget.Toast
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import androidx.navigation.fragment.findNavController
import com.mertazat.ratepulse.RatePulseApp
import com.mertazat.ratepulse.databinding.FragmentCreateAlertBinding

class CreateAlertFragment : Fragment() {

    private var _binding: FragmentCreateAlertBinding? = null
    private val binding get() = _binding!!
    private lateinit var viewModel: AlertsViewModel

    private val currencies = listOf("USD", "EUR", "TRY", "GBP", "JPY", "AUD", "CAD", "CHF", "CNY")

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?
    ): View {
        _binding = FragmentCreateAlertBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        val appContainer = (requireActivity().application as RatePulseApp).container
        viewModel = ViewModelProvider(
            this,
            AlertsViewModel.Factory(appContainer.alertRepository)
        )[AlertsViewModel::class.java]

        val adapter = ArrayAdapter(requireContext(), android.R.layout.simple_spinner_item, currencies)
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
        binding.spinnerFrom.adapter = adapter
        binding.spinnerTo.adapter = adapter
        binding.spinnerFrom.setSelection(0)      // USD
        binding.spinnerTo.setSelection(2)         // TRY

        binding.btnCreate.setOnClickListener {
            val from = binding.spinnerFrom.selectedItem.toString()
            val to = binding.spinnerTo.selectedItem.toString()
            val rateStr = binding.etTargetRate.text.toString().trim()
            val direction = if (binding.rbAbove.isChecked) "above" else "below"

            if (from == to) {
                Toast.makeText(requireContext(), "Farklı dövizler seçin", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }
            if (rateStr.isEmpty()) {
                Toast.makeText(requireContext(), "Hedef kuru girin", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }

            val rate = rateStr.toDoubleOrNull()
            if (rate == null || rate <= 0) {
                Toast.makeText(requireContext(), "Geçerli bir kur değeri girin", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }

            viewModel.createAlert(from, to, rate, direction)
        }

        viewModel.isLoading.observe(viewLifecycleOwner) { loading ->
            binding.btnCreate.isEnabled = !loading
            binding.progressBar.visibility = if (loading) View.VISIBLE else View.GONE
        }

        viewModel.message.observe(viewLifecycleOwner) { msg ->
            msg ?: return@observe
            Toast.makeText(requireContext(), msg, Toast.LENGTH_SHORT).show()
            if (msg == "Alarm oluşturuldu") findNavController().popBackStack()
        }
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
