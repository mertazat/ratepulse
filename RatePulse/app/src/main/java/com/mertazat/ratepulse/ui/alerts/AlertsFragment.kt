package com.mertazat.ratepulse.ui.alerts

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import androidx.navigation.fragment.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import com.mertazat.ratepulse.R
import com.mertazat.ratepulse.RatePulseApp
import com.mertazat.ratepulse.databinding.FragmentAlertsBinding

class AlertsFragment : Fragment() {

    private var _binding: FragmentAlertsBinding? = null
    private val binding get() = _binding!!
    private lateinit var viewModel: AlertsViewModel
    private lateinit var adapter: AlertAdapter

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?
    ): View {
        _binding = FragmentAlertsBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        val appContainer = (requireActivity().application as RatePulseApp).container
        viewModel = ViewModelProvider(
            this,
            AlertsViewModel.Factory(appContainer.alertRepository)
        )[AlertsViewModel::class.java]

        adapter = AlertAdapter { alert ->
            AlertDialog.Builder(requireContext())
                .setTitle("Alarmı Sil")
                .setMessage("${alert.fromCurrency}/${alert.toCurrency} alarmını silmek istediğinizden emin misiniz?")
                .setPositiveButton("Sil") { _, _ -> viewModel.deleteAlert(alert.id) }
                .setNegativeButton("İptal", null)
                .show()
        }

        binding.recyclerAlerts.layoutManager = LinearLayoutManager(requireContext())
        binding.recyclerAlerts.adapter = adapter

        binding.fabCreateAlert.setOnClickListener {
            findNavController().navigate(R.id.action_alertsFragment_to_createAlertFragment)
        }

        viewModel.alerts.observe(viewLifecycleOwner) { alerts ->
            adapter.submitList(alerts)
            binding.tvEmptyState.visibility = if (alerts.isEmpty()) View.VISIBLE else View.GONE
        }

        viewModel.isLoading.observe(viewLifecycleOwner) { loading ->
            binding.progressBar.visibility = if (loading) View.VISIBLE else View.GONE
        }

        viewModel.message.observe(viewLifecycleOwner) { msg ->
            msg ?: return@observe
            Toast.makeText(requireContext(), msg, Toast.LENGTH_SHORT).show()
        }
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}
