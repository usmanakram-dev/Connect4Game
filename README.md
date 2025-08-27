# Connect 4 Game - .NET MAUI Bluetooth Multiplayer

## 🎮 Introduction

Welcome to **Connect 4 Game**, a modern and feature-rich implementation of the classic Connect Four board game built with **.NET MAUI**! This cross-platform mobile application brings the timeless strategy game to your fingertips with cutting-edge **Bluetooth Low Energy (BLE)** multiplayer capabilities, stunning visual design, and seamless gameplay experience.

Connect 4 Game combines traditional gameplay mechanics with modern mobile technology, offering both local multiplayer and wireless Bluetooth connectivity. Whether you're playing with a friend on the same device or connecting wirelessly across the room, this app delivers smooth, responsive, and engaging gameplay on Android, iOS, and Windows platforms.

## ✨ Key Features

### 🎨 **Beautiful UI Design**
- **Modern Dark Theme**: Sleek dark interface with vibrant accent colors for optimal visual appeal
- **Unicode Emoji-Based Design**: No images used - entirely built with Unicode emojis and symbols for crisp, scalable graphics
- **Responsive Layout**: Adapts seamlessly to different screen sizes and orientations
- **Smooth Animations**: Fluid transitions and visual feedback for enhanced user experience
- **Platform-Specific Fonts**: Optimized emoji and symbol rendering across Windows, Android, and iOS

### 📶 **Advanced Bluetooth Low Energy (BLE) Multiplayer**
Our implementation leverages **Bluetooth Low Energy (BLE)** technology, providing superior advantages over traditional Bluetooth:

#### **BLE Technology Benefits:**
- **🔋 Ultra-Low Power Consumption**: Enables extended battery life during gameplay sessions
- **💰 Cost-Effective**: Efficient protocol reduces device resource usage
- **📱 Broad Mobile Compatibility**: Native support across modern smartphones and tablets
- **⚡ Fast Connection Times**: Quick device discovery and pairing process
- **📡 Extended Wireless Range**: Reliable connectivity up to 50+ meters in optimal conditions
- **🔒 Secure Communication**: Built-in encryption and secure pairing protocols

#### **Multiplayer Features:**
- **Host & Join Games**: Easy game hosting and discovery system
- **Real-Time Synchronization**: Instant move updates across connected devices
- **Automatic Device Discovery**: Seamless scanning for nearby games
- **Connection Management**: Robust handling of connection states and recovery

### 🎯 **Core Game Features**
- **Classic Connect 4 Gameplay**: Traditional 6×7 grid with authentic rules
- **Intelligent Win Detection**: Comprehensive checking for horizontal, vertical, and diagonal wins
- **Advanced Game State Management**: Robust tracking of game progress and player turns
- **Turn-Based Mechanics**: Clear visual indicators for current player and available moves
- **Dynamic Button Colors**: Drop buttons change color to match the current player's pieces
- **Perfect Column Alignment**: Precisely aligned drop buttons with game board columns
- **Reset & Leave Game Options**: Full game control and easy navigation

### 🎨 **Visual Design Philosophy**
- **Zero Images Policy**: Entire interface built using Unicode symbols and emojis
- **Scalable Graphics**: Vector-based symbols ensure crisp display on all screen densities
- **Platform-Optimized Fonts**: Native emoji fonts for each platform (Segoe UI Emoji on Windows, Noto Color Emoji on Android, Apple Color Emoji on iOS)
- **Accessibility-Friendly**: High contrast colors and clear visual hierarchies
- **Consistent Branding**: Unified color scheme and visual language throughout the app

## 🕹️ How to Play

### Quick Play (Local Mode)
1. Launch the Connect 4 Game app
2. Tap **"▶️ Start Local Game"** for immediate gameplay
3. Pass the device between players
4. Take turns dropping pieces by tapping the column drop buttons (⬇️)
5. First to connect 4 pieces wins!

### Bluetooth Multiplayer

#### Host a Game
1. Tap **"📶 Host Bluetooth Game"**
2. Enter your player name
3. Tap **"🚀 Start Hosting"**
4. Share your game name with the other player
5. Wait for connection and start playing!

#### Join a Game
1. Tap **"🔍 Scan for Games"**
2. Wait for nearby games to appear in the list
3. Select the desired game
4. Tap **"Join"** to connect
5. Game begins automatically once connected!

## 📋 Game Rules

- **Objective**: Connect 4 pieces of your color in a row (horizontal, vertical, or diagonal)
- **Gameplay**: Players alternate turns dropping colored pieces into columns
- **Gravity Effect**: Pieces fall to the lowest available position in the selected column
- **Win Conditions**: First player to achieve 4 in a row wins
- **Draw Condition**: If the board fills completely without a winner, the game ends in a draw
- **Turn Indicators**: Drop buttons change color to show whose turn it is (Red for Player 1, Blue for Player 2)

## ⚙️ Technical Implementation

### Architecture & Design Patterns
- **MVVM Pattern**: Clean separation of Model, View, and ViewModel layers
- **Dependency Injection**: Modular service architecture with .NET MAUI's built-in DI container
- **Plugin.BLE**: Robust Bluetooth Low Energy communication framework
- **CommunityToolkit.Mvvm**: Modern MVVM helpers with source generators and relay commands
- **Observable Properties**: Reactive UI updates using INotifyPropertyChanged

### Key Components

#### **Models Layer**
- **`GameBoard`**: Core game logic, win detection algorithms, and board state management
- **`GameSession`**: Complete game state tracking including players, current turn, and game status
- **`PlayerType`**: Enumeration for player identification and game piece types
- **`GameMove`**: Move data structure for Bluetooth transmission and game history

#### **Services Layer**
- **`IBluetoothService`**: Bluetooth communication abstraction with device discovery and data transmission
- **`IGameService`**: Game logic orchestration and state management interface
- **`DemoGameService`**: Local testing implementation for development and single-device gameplay
- **`BluetoothGameService`**: Production Bluetooth implementation for real multiplayer functionality

#### **ViewModels Layer**
- **`MainMenuViewModel`**: Main menu functionality, game hosting, and device scanning
- **`GameViewModel`**: Game board management, move processing, and UI state coordination

### Bluetooth Low Energy Protocol

Our custom BLE protocol uses efficient JSON messaging for real-time game synchronization:

```json
{
  "Type": "GameMove",
  "Move": {
    "Column": 3,
    "Player": "Player1",
    "Timestamp": "2024-01-01T12:00:00Z"
  },
  "GameState": "InProgress",
  "CurrentPlayer": "Player2",
  "BoardState": "..." 
}
```

**Protocol Features:**
- **Lightweight Messaging**: Minimal data transfer for optimal BLE performance
- **State Synchronization**: Complete game state sharing between devices
- **Error Recovery**: Robust handling of connection interruptions
- **Version Compatibility**: Future-proof protocol design

## 📱 Platform Support

- 🤖 **Android** (API 21+ / Android 5.0+)
- 🍎 **iOS** (11.0+)
- 🪟 **Windows** (10.0.17763.0+)

## 🔐 Required Permissions

### Android Permissions
```xml
<!-- Basic Bluetooth permissions -->
<uses-permission android:name="android.permission.BLUETOOTH" />
<uses-permission android:name="android.permission.BLUETOOTH_ADMIN" />

<!-- Location permissions (required for BLE device discovery) -->
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />

<!-- Android 12+ (API 31+) BLE permissions -->
<uses-permission android:name="android.permission.BLUETOOTH_SCAN" />
<uses-permission android:name="android.permission.BLUETOOTH_ADVERTISE" />
<uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />

<!-- Hardware feature requirements -->
<uses-feature android:name="android.hardware.bluetooth" android:required="true" />
<uses-feature android:name="android.hardware.bluetooth_le" android:required="false" />
```

### iOS Permissions
- **Bluetooth Usage Description**: Required entry in Info.plist for BLE access
- **Privacy Usage Descriptions**: Clear user-facing explanations for Bluetooth usage

## 🛠️ Development Setup

### Prerequisites
- **.NET 8.0 SDK** or later
- **Visual Studio 2022** (17.8+) or **Visual Studio Code** with .NET MAUI extension
- **Platform-Specific Tools**:
  - **Android**: Android SDK (API 21+)
  - **iOS**: Xcode 14+ (macOS only)
  - **Windows**: Windows 10 SDK (17763+)

### Build and Run
1. **Clone the Repository**
   ```bash
   git clone [repository-url]
   cd Connect4Game
   ```

2. **Open in Visual Studio**
   ```bash
   start Connect4Game.sln
   ```

3. **Select Target Platform**
   - Choose Android, iOS, or Windows from the platform dropdown

4. **Build and Deploy**
   - Press F5 or click "Start Debugging"

### Development Modes

#### **Debug Mode**
- Uses `DemoGameService` for local testing
- No Bluetooth requirement for development
- Enhanced logging and diagnostic information
- Faster iteration during development

#### **Release Mode**
- Activates full Bluetooth functionality
- Production-ready BLE services
- Optimized performance and battery usage
- Real device testing capabilities

## 🎨 Customization Options

### **Color Themes**
Modify `Connect4Game/Resources/Styles/AppStyles.xaml` to customize the visual theme:

```xml
<!-- Background Colors -->
<Color x:Key="PrimaryBackgroundColor">#1a1a2e</Color>
<Color x:Key="SecondaryBackgroundColor">#16213e</Color>

<!-- Player Colors -->
<Color x:Key="Player1Color">#FF4444</Color>
<Color x:Key="Player2Color">#4444FF</Color>

<!-- Accent Colors -->
<Color x:Key="PrimaryAccentColor">#e94560</Color>
<Color x:Key="SecondaryAccentColor">#FFD700</Color>
```

### **Game Logic Customization**
Adjust game parameters in `Connect4Game/Models/GameModels.cs`:

```csharp
public static class GameBoard
{
    public const int Rows = 6;        // Board height
    public const int Columns = 7;     // Board width
    public const int WinLength = 4;   // Pieces needed to win
}
```

### **Unicode Symbols**
All visual elements use Unicode symbols for universal compatibility:
- **Drop Buttons**: ⬇️ (U+2193 Downwards Arrow)
- **Player Pieces**: ⚫ (U+25CF Black Circle) and O (U+004F Latin Capital Letter O)
- **Menu Icons**: ▶️ (U+25B6 Play), ⚡ (U+26A1 Lightning), ⭕ (U+25CB Circle)

## 🔧 Troubleshooting

### **Bluetooth Connectivity Issues**
1. **Enable Bluetooth**: Ensure Bluetooth is active on both devices
2. **Grant Permissions**: Verify all required permissions are granted
3. **Check Range**: Maintain reasonable distance between devices (under 30 meters)
4. **Restart Services**: Close and reopen the app to reset Bluetooth services
5. **Clear Cache**: Clear Bluetooth cache in Android system settings

### **Performance Optimization**
1. **Close Background Apps**: Reduce interference from other Bluetooth applications
2. **Battery Optimization**: Disable battery optimization for the app on Android
3. **Update Devices**: Ensure both devices have recent OS updates
4. **Network Interference**: Move away from Wi-Fi routers and other 2.4GHz devices

### **UI Rendering Issues**
1. **Font Support**: Verify platform supports Unicode emoji rendering
2. **Screen Density**: Test on different screen sizes and DPI settings
3. **Platform Differences**: Some emoji may render differently across platforms

## 🤝 Contributing

We welcome contributions from the community! Here's how to get involved:

1. **Fork the Repository** on your preferred Git platform
2. **Create a Feature Branch**: `git checkout -b feature/amazing-feature`
3. **Make Your Changes**: Implement your improvements
4. **Add Tests**: Include unit tests for new functionality
5. **Update Documentation**: Modify README and code comments as needed
6. **Submit a Pull Request**: Describe your changes and their benefits

### **Contribution Guidelines**
- Follow existing code style and conventions
- Write descriptive commit messages
- Test on multiple platforms when possible
- Update documentation for new features
- Respect the Unicode-only design philosophy

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for complete details.

## 🙏 Acknowledgments

- **[Plugin.BLE](https://github.com/dotnet-bluetooth-le/dotnet-bluetooth-le)** - Excellent Bluetooth Low Energy framework for .NET
- **[CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)** - Powerful MVVM helpers and source generators
- **[.NET MAUI Team](https://github.com/dotnet/maui)** - Outstanding cross-platform framework
- **Unicode Consortium** - Universal character standards enabling emoji-based design
- **Open Source Community** - Inspiration and shared knowledge

---

**Ready to play?** Download Connect 4 Game and experience the perfect blend of classic gameplay and modern technology! 🎮🚀
