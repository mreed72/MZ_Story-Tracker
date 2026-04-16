#  MZ Story Tracker v1.2

**A specialized narrative management solution optimized for RPG Maker MZ and legacy engines.**

While specifically designed to complement the workflow of **RPG Maker MZ** (and earlier versions like MV, VX Ace, and XP), MZ Story Tracker is a versatile tool for any game development platform. It provides a centralized hub to bridge the gap between your creative brainstorming and your database—keeping your maps, events, and narrative beats streamlined, organized, and secure.


---

##  Key Features

*   **Dynamic Data Indexing**: Instantaneously retrieve, filter, and edit records through an intuitive, real-time search interface.
*   **XML-Driven Backend**: High-performance data management ensures your story beats are stored in a portable, human-readable format.
*   **State Persistence**: Never lose your place. The application remembers your customized UI layout and window position across sessions.
*   **Robust Error Logging**: Integrated diagnostic systems automatically track irregularities, ensuring maximum reliability for your critical data.
*   **Modular Design**: Stores all mission-critical assets in a dedicated local directory for effortless backup and migration.

##  Built With

*   **Language**: Visual Basic .NET
*   **Data Storage**: XML (LINQ to XML)
*   **Platform**: Windows Forms (.NET Framework)

##  Installation & Setup

1.  Navigate to the **Releases** section of this repository.
2.  Download the latest `MZ_StoryTracker.zip`.
3.  Extract the contents to a folder on your computer.
4.  Run `MZ_StoryTracker.exe`.
    *   *Note: On first run, the app will automatically create its data directory at `C:\MZ_Story Tracker\`.*

##  How to Use

1.  **Adding Records**: Enter your story title, map location, and event details. Use the "Notes" section for deeper context and click **Save**.
2.  **Searching**: Use the search bar above the list box to filter your stories in real-time as you type.
3.  **Managing Data**: Click any item in the list to load its data into the form. You can then update the details and click **Save** or use **Delete** to remove the record.
4.  **Testing**: Use the temporary **Fill Sample** button to quickly generate 30 records to test the scrolling and search features.

## 📁 File Structure
*   `C:\MZ_Story Tracker\Data\dat.xml`: The primary database for all story records.
*   `C:\MZ_Story Tracker\settings.xml`: Stores personalized application configurations and window location.
*   `C:\MZ_Story Tracker\Data\errorlog.txt`: Automatically generated logs for troubleshooting.

---

## 📜 License
This project is open-source. See the `LICENSE` file for more information.

## 👤 Author
**Marcus Reed**
*Transforming narrative chaos into organized legends.*
