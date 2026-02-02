# Neural Network ECS Simulation

Experimental **neural network simulation project** built with **ECS**, **GPU instancing**, and multiple performance-oriented techniques to support large-scale agent simulation.

The project uses **neural networks combined with genetic algorithms** to train agents to develop specific behaviors through **trial and error**.

This project started as a **2024 Image Campus project** and later evolved into an independent experiment.

---

## Project Overview

The simulation focuses on an ecosystem-style scenario with multiple agent types:

- **Herbivores**  
  Learn to search for food, avoid predators, and survive efficiently.

- **Carnivores**  
  Learn hunting behaviors and target selection.

- **Scavengers**  
  Learn to locate remains, avoid direct superposition, and optimize mantain a distance from the target.

Each agent type evolves distinct behaviors based on its role in the ecosystem.

---

## Learning Approach

- Neural networks control agent decision-making
- Genetic algorithms evolve network parameters over generations
- Behaviors emerge through trial and error, not hardcoded rules
- Fitness is evaluated based on survival and role-specific goals

---

## Technical Highlights

- ECS architecture for high-performance simulation
- GPU instancing to render thousands of agents efficiently
- Pathfinding using A*
- Optimized update loops for large populations
- Data-oriented design to minimize memory overhead

---

## License

MIT
