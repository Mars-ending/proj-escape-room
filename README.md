# Project Selene: Lunar Reconstruction 🌖

> **🌌 Live 3D Presentation / 在线互动演示:** > **[Click Here to View the Interactive Pitch Deck](https://Mars-ending.github.io/proj-escape-room/)**
> *(Tip: Press `F11` for full-screen mode and use Arrow Keys to navigate / 建议按 F11 全屏并使用方向键翻页)*

## 📖 Project Overview | 项目简介

**Project Selene** is a Virtual Reality (VR) simulation focused on extraterrestrial resource management and HCI (Human-Computer Interaction). Developed using Unity and MRTK3, it simulates a closed-loop resource management cycle where players navigate the solar system to harvest materials and reconstruct the lunar core.

本项目是一个基于 VR 的星际资源采集与重构模拟系统。玩家将扮演“方舟号”空间站指挥官，利用相位虫洞技术穿梭太阳系，结合各大行星的物理特性加工资源，最终完成修复月球核心的任务。

## 🗂️ Repository Structure | 仓库结构

This repository contains both the Unity source code and the WebGL-based interactive presentation:
本仓库同时包含 Unity 工程源码与基于 WebGL 的 3D 互动路演网页：

* **`/` (Root Directory):** The core Unity project (Unity 2022 LTS + URP). Contains all scripts, prefabs, and assets for the VR simulation.
* **`/docs`:** The Three.js & GSAP based 3D presentation website. Served live via GitHub Pages.

## 🛠️ Technical Stack | 技术栈

**VR Development (Game Engine):**
* Engine: Unity 2022 LTS (Universal Render Pipeline)
* Interaction Framework: Mixed Reality Toolkit 3 (MRTK3)
* Target Platform: Meta Quest / OpenXR compatible HMDs

**Web Presentation (Pitch Deck):**
* Graphics: Three.js (WebGL)
* Animation: GSAP
* Features: Custom bloom shaders, orbital mechanics simulation, glassmorphism UI.

## 🚀 How to Run | 运行指南

### 1. Web Presentation
Simply visit the [Live Link](https://Mars-ending.github.io/proj-escape-room/) on any modern desktop browser. No installation required.

### 2. Unity VR Project
1. Clone this repository: `git clone https://github.com/Mars-ending/proj-escape-room.git`
2. Open the project folder using **Unity Hub** (Ensure Unity 2022 LTS is installed).
3. Connect your VR headset via Oculus Link or Virtual Desktop.
4. Open the main scene in `Assets/Scenes` and press Play.

---
*Designed & Developed by [Your Name/Mars-ending] | 202X*
