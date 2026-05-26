package com.mertazat.ratepulse.ui.profile

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import androidx.navigation.fragment.findNavController
import com.google.firebase.auth.FirebaseAuth
import com.mertazat.ratepulse.R
import com.mertazat.ratepulse.RatePulseApp
import com.mertazat.ratepulse.databinding.FragmentProfileBinding

class ProfileFragment : Fragment() {

    private var _binding: FragmentProfileBinding? = null
    private val binding get() = _binding!!
    private lateinit var viewModel: ProfileViewModel

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?
    ): View {
        _binding = FragmentProfileBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        val appContainer = (requireActivity().application as RatePulseApp).container
        viewModel = ViewModelProvider(
            this,
            ProfileViewModel.Factory(appContainer.userRepository)
        )[ProfileViewModel::class.java]

        binding.btnSave.setOnClickListener {
            val name = binding.etDisplayName.text.toString().trim()
            if (name.isEmpty()) {
                Toast.makeText(requireContext(), "İsim boş olamaz", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }
            val currencies = binding.etPreferredCurrencies.text.toString()
                .split(",")
                .map { it.trim().uppercase() }
                .filter { it.isNotEmpty() }
            viewModel.updateProfile(name, currencies)
        }

        binding.btnLogout.setOnClickListener {
            FirebaseAuth.getInstance().signOut()
            findNavController().navigate(R.id.action_profileFragment_to_loginFragment)
        }

        viewModel.profile.observe(viewLifecycleOwner) { profile ->
            profile ?: return@observe
            binding.tvEmail.text = profile.email
            binding.etDisplayName.setText(profile.displayName)
            binding.tvRole.text = "Rol: ${profile.role}"
            binding.etPreferredCurrencies.setText(profile.preferredCurrencies.joinToString(", "))
            binding.tvMemberSince.text = "Üyelik: ${profile.createdAt.take(10)}"
        }

        viewModel.isLoading.observe(viewLifecycleOwner) { loading ->
            binding.progressBar.visibility = if (loading) View.VISIBLE else View.GONE
            binding.btnSave.isEnabled = !loading
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
