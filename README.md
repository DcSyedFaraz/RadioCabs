
# RadioCabs

**RadioCabs** is a desktop application developed in C# using Windows Forms, designed to streamline taxi dispatch operations. It facilitates efficient management of bookings, drivers, vehicles, and fare calculations, making it an ideal solution for small to medium-sized taxi companies.

## 🚀 Features

* **Booking Management**: Create, update, and track taxi bookings with ease.
* **Driver & Vehicle Management**: Maintain comprehensive records of drivers and their assigned vehicles.
* **Fare Calculation**: Automatically compute fares based on distance and time parameters.
* **Real-time Dispatching**: Assign available drivers to bookings promptly.
* **User-friendly Interface**: Intuitive UI built with Windows Forms for seamless navigation.

## 🛠️ Tech Stack

* **Frontend**: C# with Windows Forms
* **Backend**: .NET Framework
* **Database**: SQL Server

## 🖥️ Screenshots

*Include relevant screenshots here to showcase the application's interface and features.*

## 📦 Installation

1. **Clone the Repository**:

   ```bash
   git clone https://github.com/DcSyedFaraz/RadioCabs.git
   ```

2. **Open in Visual Studio**:

   * Navigate to the cloned directory.
   * Open the `E-Project.sln` solution file in Visual Studio.

3. **Restore NuGet Packages**:

   * In Visual Studio, go to `Tools` > `NuGet Package Manager` > `Manage NuGet Packages for Solution...`.
   * Restore any missing packages.

4. **Configure the Database**:

   * Ensure SQL Server is installed and running.
   * Create a new database named `RadioCabsDB`.
   * Update the connection string in the application's configuration file (`App.config`) to point to your SQL Server instance.

5. **Build and Run**:

   * Build the solution (`Build` > `Build Solution`).
   * Run the application (`Debug` > `Start Debugging`).

## 🧾 Usage

Upon launching the application:

* **Login**: Authenticate using your credentials.
* **Dashboard**: Access an overview of current bookings and driver statuses.
* **Bookings**: Create new bookings or manage existing ones.
* **Drivers**: Add or update driver information.
* **Vehicles**: Manage vehicle details and assignments.
* **Reports**: Generate reports on bookings, earnings, and driver performance.

## 📂 Project Structure

```plaintext
RadioCabs/
├── E-Project.sln
├── E-Project/
│   ├── Forms/
│   │   ├── LoginForm.cs
│   │   ├── DashboardForm.cs
│   │   └── ...
│   ├── Models/
│   │   ├── Booking.cs
│   │   ├── Driver.cs
│   │   └── Vehicle.cs
│   ├── Services/
│   │   ├── BookingService.cs
│   │   └── ...
│   ├── App.config
│   └── Program.cs
└── README.md
```

## 🤝 Contributing

Contributions are welcome! If you'd like to enhance the application or fix issues:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/YourFeature`).
3. Commit your changes (`git commit -m 'Add YourFeature'`).
4. Push to the branch (`git push origin feature/YourFeature`).
5. Open a Pull Request.

## 📄 License

This project is licensed under the [MIT License](LICENSE).

## 📧 Contact

For any inquiries or support, please contact [your.email@example.com](mailto:your.email@example.com).

