# Unity Developer Intern Assessment

This project is a **custom Unity Editor Window** designed to list, filter, and manage GameObjects within the scene efficiently.

## 📌 Features

- **GameObject Listing**: Displays all GameObjects in the current scene.
- **Search & Filter**: Search GameObjects by name and filter by component (MeshRenderer, Collider, Rigidbody).
- **Edit Transform**: Modify Position, Rotation, and Scale values directly from the editor.
- **Undo/Redo Support**: Revert changes using standard Unity Undo/Redo functionality.
- **Multi-Selection & Batch Editing**: Modify multiple GameObjects simultaneously.
- **Toggle Active State**: Enable/Disable GameObjects via a checkbox in the list.

## 🛠️ Setup & Installation

1. Clone this repository:
   ```sh
   git clone https://github.com/yigitsariyar23/rapsodo-intern-assignment

   ```
2. Open the project in **Unity 6000.0.40f1**.
3. Navigate to `Tools > Custom Editor Window` in the Unity Editor to open the custom editor window.

## 🎮 Usage Instructions

1. Open **Tools > Custom Editor Window** from the Unity menu.
2. Click **Initialize w/ grouping** or **Initialize w/o grouping** to load GameObjects into the manager.
3. Use the **Search** bar to find GameObjects by name.
4. Enable **Mesh Renderer, Collider, or Rigidbody** checkboxes to filter GameObjects by component.
5. Use the **Active State** checkboxes to enable or disable GameObjects.
6. Select one or multiple GameObjects by clicking their checkboxes.
7. Modify **Position, Rotation, and Scale** values in the **Batch Transform Edit** section to apply changes.
8. Use the **Select Component** dropdown to add or remove a component from all selected GameObjects.
9. Click **Refresh List** to update the GameObject list after scene modifications.
10. Use **Undo (Ctrl+Z) / Redo (Ctrl+Y)** to revert changes.

## 📁 Project Structure

```
📂 UnityProject/
├── 📂 Assets/
│   ├── 📂 Editor/  # Contains GameObjectManagerWindow.cs
│   ├── 📂 Scenes/  # Scene setup with required GameObjects
├── 📂 ProjectSettings/
├── 📂 Packages/
├── README.md  👈 (This file)
```

## 📜 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for more details.
