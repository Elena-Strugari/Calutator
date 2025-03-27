# Calutator – WPF Desktop Calculator App

This project is a **WPF-based desktop calculator** built in C#. It offers a simple yet customizable interface for performing arithmetic operations, while also persisting user preferences between sessions.

## Features

- Standard arithmetic operations
- Digit grouping (thousands separator) toggle
- Mode selection with persistence (e.g., Standard, Scientific)
- User settings saved in a local `settings.txt` file
- Intuitive WPF UI design using XAML
- Lightweight and fast desktop application

## Technologies Used

- **C#**
- **WPF (Windows Presentation Foundation)**
- .NET Framework
- XAML for UI definition
- File I/O for settings persistence

## Key Components

- `App.xaml`: Entry point and startup configuration
- `MainWindow.xaml`: Main UI of the calculator (not shown here)
- `AppSettings.cs`: Logic for saving and loading user preferences
