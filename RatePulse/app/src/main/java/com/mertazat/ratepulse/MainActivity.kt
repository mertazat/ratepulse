package com.mertazat.ratepulse

import android.Manifest
import android.content.pm.PackageManager
import android.os.Build
import android.os.Bundle
import android.view.View
import androidx.activity.result.contract.ActivityResultContracts
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.core.content.ContextCompat
import androidx.navigation.fragment.NavHostFragment
import androidx.navigation.ui.setupWithNavController
import com.mertazat.ratepulse.databinding.ActivityMainBinding

class MainActivity : AppCompatActivity() {

    private lateinit var binding: ActivityMainBinding

    private val notificationPermissionLauncher =
        registerForActivityResult(ActivityResultContracts.RequestPermission()) { granted ->
            // İzin sonucu — herhangi bir UI güncellemesi gerekmiyorsa sessizce geç
        }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)

        requestNotificationPermission()

        val navHostFragment = supportFragmentManager
            .findFragmentById(R.id.nav_host_fragment) as NavHostFragment
        val navController = navHostFragment.navController

        binding.bottomNav.setupWithNavController(navController)

        navController.addOnDestinationChangedListener { _, destination, _ ->
            val hideBottomNav = destination.id in listOf(
                R.id.loginFragment,
                R.id.registerFragment
            )
            binding.bottomNav.visibility = if (hideBottomNav) View.GONE else View.VISIBLE
        }
    }

    private fun requestNotificationPermission() {
        // POST_NOTIFICATIONS runtime izni yalnızca Android 13+ (API 33) için gerekli
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.TIRAMISU) return

        when {
            // İzin zaten verilmiş
            ContextCompat.checkSelfPermission(
                this, Manifest.permission.POST_NOTIFICATIONS
            ) == PackageManager.PERMISSION_GRANTED -> return

            // Kullanıcı daha önce reddetti — gerekçe diyaloğu göster
            shouldShowRequestPermissionRationale(Manifest.permission.POST_NOTIFICATIONS) -> {
                AlertDialog.Builder(this)
                    .setTitle("Bildirim İzni")
                    .setMessage(
                        "Döviz kur alarmlarınız tetiklendiğinde anında bildirim alabilmek için " +
                        "bildirim iznine ihtiyaç vardır."
                    )
                    .setPositiveButton("İzin Ver") { _, _ ->
                        notificationPermissionLauncher.launch(Manifest.permission.POST_NOTIFICATIONS)
                    }
                    .setNegativeButton("Hayır", null)
                    .show()
            }

            // İlk kez soruluyor
            else -> {
                notificationPermissionLauncher.launch(Manifest.permission.POST_NOTIFICATIONS)
            }
        }
    }
}
