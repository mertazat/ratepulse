package com.mertazat.ratepulse.ui.alerts

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.DiffUtil
import androidx.recyclerview.widget.ListAdapter
import androidx.recyclerview.widget.RecyclerView
import com.mertazat.ratepulse.R
import com.mertazat.ratepulse.data.model.RateAlert
import com.mertazat.ratepulse.databinding.ItemAlertBinding

class AlertAdapter(
    private val onDelete: (RateAlert) -> Unit
) : ListAdapter<RateAlert, AlertAdapter.AlertViewHolder>(DiffCallback()) {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): AlertViewHolder {
        val binding = ItemAlertBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return AlertViewHolder(binding)
    }

    override fun onBindViewHolder(holder: AlertViewHolder, position: Int) {
        holder.bind(getItem(position))
    }

    inner class AlertViewHolder(private val binding: ItemAlertBinding) :
        RecyclerView.ViewHolder(binding.root) {

        fun bind(alert: RateAlert) {
            binding.apply {
                tvPair.text = "${alert.fromCurrency}/${alert.toCurrency}"
                tvTargetRate.text = "Hedef: %.4f".format(alert.targetRate)
                tvDirection.text = if (alert.direction == "above") "↑ Üstünde" else "↓ Altında"
                tvDirectionArrow.text = if (alert.direction == "above") "↑" else "↓"

                tvStatus.text = if (alert.isActive) "Aktif" else "Tetiklendi"
                tvStatus.setBackgroundResource(
                    if (alert.isActive) R.drawable.bg_status_active else R.drawable.bg_status_triggered
                )

                btnDelete.setOnClickListener { onDelete(alert) }
            }
        }
    }

    class DiffCallback : DiffUtil.ItemCallback<RateAlert>() {
        override fun areItemsTheSame(oldItem: RateAlert, newItem: RateAlert) = oldItem.id == newItem.id
        override fun areContentsTheSame(oldItem: RateAlert, newItem: RateAlert) = oldItem == newItem
    }
}
