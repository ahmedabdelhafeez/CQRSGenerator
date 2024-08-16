# CQRS Files Generator

## Overview

The **CQRS Files Generator** is a powerful Visual Studio extension designed to streamline the development process by automatically generating the necessary files for implementing the CQRS (Command Query Responsibility Segregation) design pattern in your .NET projects. This extension helps you maintain a clear separation between command and query responsibilities, improving the scalability and maintainability of your applications.

## Features

- **Command and Query Separation**: Automatically generate Command, Query, Handler, and Response files.
- **Namespace Management**: Ensures the generated files are placed in the correct namespaces based on the selected folder within your project.
- **Easy to Use**: Seamlessly integrates with Visual Studio, allowing you to generate files directly from the Solution Explorer context menu.

## Installation

You can install the **CQRS Files Generator** extension directly from the Visual Studio Marketplace. 

[![Install from Visual Studio Marketplace](https://img.shields.io/badge/Install%20from-Marketplace-blue.svg)](https://marketplace.visualstudio.com/items?itemName=AhmedAbdelHafeez.CQRSFilesGenerator&ssr=false#overview)

### Steps to Install:

1. Visit the [Visual Studio Marketplace page](https://marketplace.visualstudio.com/items?itemName=AhmedAbdelHafeez.CQRSFilesGenerator&ssr=false#overview).
2. Click on the **"Download"** button.
3. Follow the instructions to install the extension into your Visual Studio environment.
4. Restart Visual Studio to complete the installation.

Once installed, the extension will be available in the Solution Explorer context menu, allowing you to generate CQRS files quickly and efficiently.

## Usage

1. **Open Your Project**: Open your .NET project in Visual Studio.
2. **Select Folder or Project**: In Solution Explorer, right-click on the folder or project where you want to create CQRS files.
3. **Generate Files**: Choose the appropriate command (e.g., "Create Command Files") from the context menu.
4. **Provide Input**: If prompted, provide the necessary information, such as the name of the command or query.
5. **View Generated Files**: The required files will be generated in the selected location with the appropriate namespaces and basic structure.

## Create Types

The following  types can be generated:

- **CQRS**:Generates all the necessary components for implementing the CQRS pattern, including Create, Read, Update, and Delete (CRUD) operations. It creates command and query files along with their respective handlers and response classes, ensuring that both the command and query responsibilities are clearly separated and handled efficiently.
- **Command**: Generates a command class, a command handler, and a response class. Commands represent actions that change the state of the application (such as creating or updating a record). The command handler processes these commands, while the response class defines the structure of the response returned after the command is executed.
- **Query**: Generates a query class, a query handler, and a response class. Queries are used to retrieve data without modifying the application state. The query handler processes the queries, and the response class defines the structure of the data returned by the query.
  
## Contributing

Contributions are welcome! If you have suggestions for new features or find any issues, please open an issue or submit a pull request. Follow the guidelines below:

1. Fork the repository.
2. Create a new branch (`feature/YourFeature` or `fix/YourFix`).
3. Commit your changes.
4. Push to the branch.
5. Open a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

If you have any questions or need support, please reach out through the [GitHub Issues](https://github.com/ahmedabdelhafeez/CQRSGenerator/issues) page.


## Acknowledgements

Thanks to all contributors and the open-source community for making this project possible.

---
Created by [Ahmed Abdel_Hafeez](https://github.com/ahmedabdelhafeez)
