# Procedural Content Generation in Dungeon Crawlers using Grammars and Digging Agents

## Overview

This repository contains the implementation of a procedural content generation system for dungeon crawler games, using a combination of grammar-based rules and a digger agent. The goal is to create dynamic, non-repetitive, and engaging dungeon environments in real-time as the game is played.

## Features

- **Grammar-Based Generation**: Structured and logical layout creation using predefined grammar rules.
- **Digger Agent**: A dynamic agent that carves out paths and rooms within the dungeon, adding unpredictability and exploration.
- **Hybrid Approach**: Combines the strengths of grammar-based systems with the creativity of agent-based generation for varied and complex environments.

## Table of Contents

1. [Introduction](#introduction)
2. [Project Structure](#project-structure)
3. [Installation](#installation)
4. [Usage](#usage)
5. [Methodology](#methodology)
    - [Grammar-Based Content Generation](#grammar-based-content-generation)
    - [Digging Agent](#digging-agent)
    - [Combining the Methods](#combining-the-methods)
6. [Results](#results)
7. [Future Work](#future-work)
8. [Contributing](#contributing)
9. [License](#license)

## Introduction

The gaming industry is continuously evolving, and so are the methods used to generate content within these games. Procedural content generation (PCG) is a method used to create game environments dynamically, without the need for manual design. This project focuses on PCG in dungeon crawlers, utilizing a combination of grammars and digging agents to produce unique, playable dungeons.

## Project Structure

```
.
├── assets/                 # Images, screenshots, and other media files
├── src/                    # Source code for the project
│   ├── grammar/            # Implementation of grammar-based generation
│   ├── agent/              # Implementation of the digging agent
│   └── ...
├── tests/                  # Unit tests for the project
├── LICENSE
└── README.md
```
## Installation

### Prerequisites

- **Unity Engine**: This project was developed using Unity 2022.3.2f1. Ensure you have this version or a compatible version installed. You can download Unity Hub and install the required version from the Unity website.
- **Git**: Make sure Git is installed on your machine for version control and to clone the repository.

### Steps

1. **Clone the Repository**
   ```bash
   git clone https://github.com/yourusername/dungeon-pcg.git
   ```

2. **Open the Project in Unity**
   - Launch **Unity Hub**.
   - Click on the **"Open"** button in Unity Hub.
   - Navigate to the directory where you cloned the repository (`dungeon-pcg`) and select the folder to open it in Unity.

3. **Install Required Packages**
   - Once the project is opened, Unity will automatically check for missing packages. If prompted to install any missing dependencies or packages, click **"Install/Update"**.
   - Ensure that all necessary packages (e.g., **Cinemachine**, **Post-processing**) are installed via the Unity Package Manager.

4. **Build and Run**
   - After the project loads, you can build the project by going to **File > Build Settings**.
   - Select your target platform (e.g., PC, Mac, Linux Standalone).
   - Click on **"Build and Run"** to compile and start the game.

5. **Customize and Generate Dungeons**
   - To generate a dungeon, you can start the game directly from the Unity Editor by clicking the **Play** button.
   - Modify grammar rules and agent parameters by navigating to the relevant scripts in the **Scripts/** directory.

## Usage

1. To generate a dungeon, run the project and walk into hallways, the dungeons will get generated as the player walks into unexplored hallways.
2. Customize the generation by modifying the grammar rules and agent parameters.

## Methodology

### Grammar-Based Content Generation

The grammar-based system uses a set of predefined rules to generate the structure of the dungeon. These rules dictate how rooms and corridors are connected, ensuring a logical and coherent layout.



### Digging Agent

The digger agent, explores the virtual space, carving out paths and rooms based on the parameters set in the grammar. This agent introduces an element of randomness, making each playthrough unique.

### Combining the Methods

By combining grammar-based rules with the dynamic behavior of the digging agent, this method achieves a balance between structured design and creative freedom, resulting in varied and engaging dungeon environments.

## Results

*Describe the outcomes of your project here. Include images, performance metrics, or gameplay videos to illustrate the effectiveness of your method.*

## Future Work

- **Enhancing Agent Intelligence**: Improve the decision-making capabilities of the digging agent.
- **Expanding Grammar Complexity**: Introduce more complex grammar rules for even more varied dungeons.
- **User Interface**: Develop a user-friendly interface for customizing and generating dungeons.

## Contributing

Contributions are welcome! Please read the [contributing guidelines](CONTRIBUTING.md) before submitting a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
