# 🌍 Environmental Air Quality Monitoring System

A LabVIEW-based environmental monitoring application for visualizing air quality monitoring stations, retrieving real-time sensor data from the OpenAQ platform, and plotting historical measurements.

The project combines **LabVIEW** and **C# (.NET)** to provide an interactive GIS interface capable of displaying monitoring stations, searching nearby sensors, and visualizing air quality measurements.

---

## ✨ Features

- 🗺️ Interactive map visualization
- 📍 Search nearby monitoring stations within a configurable radius
- 📌 Dynamic marker management
- 📊 Historical sensor data plotting
- 🔑 OpenAQ API key validation
- 🔍 Display air quality information by hovering over station markers
- 📝 Live application status log
- ❌ Remove individual markers or clear all markers
- 🔄 Communication between multiple LabVIEW VIs using queues

---

## Air Quality Parameters

The application currently supports:

- PM2.5
- PM10
- NO₂
- O₃

---

# Demo

## Map View

<img src="images/map.png" width="900">

## Station Information

<img src="images/station.png" width="900">

## Historical Graph

<img src="images/graph.png" width="900">

---

# Project Architecture

```
                    OpenAQ API
                         │
                         │
                  HTTP Requests
                         │
         ┌───────────────┴───────────────┐
         │                               │
      LabVIEW                      GMapControl.dll
       main.vi              (Custom C# Map Control)
         │                               │
         └───────────────┬───────────────┘
                         │
                  Queue Communication
                         │
                      map.vi
                         │
             Interactive GIS Interface
```

---

# Project Structure

```
Environmental_Monitoring_System/
│
├── main.vi
├── map.vi
│
├── GMapControl/
│   └── Custom C# library for map visualization
│
├── GMapViewer/
│   └── Standalone testing application
│
├── SubVIs/
│   ├── findCityv4.vi
│   ├── getSensorData.vi
│   └── graph_data.vi
│
├── Global Variables/
│   ├── API_Key
│   ├── ID_List
│   └── Sensor_List
│
└── Queues
    ├── map_queue
    └── map_data_queue
```

---

# Technologies Used

- LabVIEW 2024 Q3 (32-bit)
- C#
- .NET
- GMap.NET
- OpenAQ REST API
- JSON
- SQLite
- Queue-based communication
- HTTP Client

---

# How It Works

### 1. Launch the Application

Open **main.vi** and press **Start**.

---

### 2. Enter OpenAQ API Key

The application validates the API key before allowing access to the monitoring system.

---

### 3. Explore the Map

- Zoom using the mouse wheel
- Pan freely
- Right-click anywhere on the map
- Search nearby monitoring stations
- Select the desired search radius

Markers are automatically added for every station found.

---

### 4. View Sensor Information

Hover over a marker to display:

- PM2.5
- PM10
- NO₂
- O₃

along with the latest available measurements.

---

### 5. Plot Historical Measurements

Switch to the **Graph** tab.

Select:

- Station
- Sensor
- Start date
- End date

Press **Plot** to retrieve and display historical measurements.

---

# Installation

## Requirements

- LabVIEW 2024 Q3 (32-bit)
- OpenAQ API key
- Internet connection

Generate an API key from your OpenAQ account before running the project.

---

# Technical Challenges

During development several engineering challenges had to be solved:

- Building a .NET DLL compatible with LabVIEW
- Integrating a custom C# map control
- Queue synchronization between multiple VIs
- Parsing inconsistent JSON responses from the API
- Preventing duplicate map markers
- SQLite integration in LabVIEW
- Managing API rate limits to avoid account suspension

---

# Future Improvements

- Real-time automatic updates
- Historical trend analysis
- Pollution heatmaps
- Weather data integration
- Machine learning-based pollution prediction
- NI DAQ sensor integration
- Multi-user database support
- CSV / Excel export
- Enhanced graph visualization

---

# Repository Highlights

This project demonstrates practical experience with:

- LabVIEW application development
- Event-driven programming
- Queue-based software architecture
- GIS visualization
- REST API integration
- C#/.NET interoperability
- JSON parsing
- Software modularization

---

# Author

**Peyman Peirovifar**

M.Sc. Student  
Sustainable and Autonomous Systems  
University of Oulu

GitHub: https://github.com/PeymanPe

---

# License

This project is licensed under the MIT License.
