# Running eShopOnDapr

## Run eShopOnDapr from the CLI

The root folder of the repository contains [Docker Compose](https://docs.docker.com/compose/) files to run the solution locally. The `docker-compose.yml` file contains the definition of all the images needed to run eShopOnDapr. The `docker-compose.override.yml` file contains the base configuration for all images of the previous file.

To start eShopOnDapr from the CLI, run the following command from the root folder:

``` 
docker-compose up
```

First Docker builds the images. This can take between 10 and 30 minutes to complete, depending on the system speed.

![Docker building images](media/docker-build.png)

Once the images have been built, Docker will start the containers. You should now see the application logs in the terminal:

![Application logging](media/docker-application-output.png)

## Run eShopOnDapr from Visual Studio

Use Visual Studio to get the best F5 debugging experience. To start, open the `eShopOnDapr.sln` solution file in Visual Studio. Below you can see the full `eShopOnDapr.sln` solution opened in Visual Studio 2019:

![Visual Studio solution](media/vs-solution.png)

The solution contains a Docker Compose project. Make sure it's set as the default startup project. Right-click on the `docker-compose` node in the Project Explorer, and select the *Set as StartUp Project* menu option:

![Set Docker Compose as StartUp project](media/vs-startup-project.png)

Now you can build and run the application by pressing Ctrl+F5 or start debugging by pressing F5. You can also press the *Debug* button in the toolbar:

![Start debugging](media/vs-debug.png)

## Run eShopOnDapr from Visual Studio Code

To run eShopOnDapr from Visual Studio Code it's best to install the *C#* and *Docker* extensions.

Open the root folder in Visual Studio Code:

![Visual Studio Code](media/vscode.png)

Open the command palette (Ctrl+Shift+P/Command+Shift+P) and select the *Docker: Compose Up* command:

![Docker Compose Up](media/vscode-compose.png)

Visual Studio Code will run docker compose to build and start the containers.

> Do *not* select *Compose Up* from the context menu of the `docker-compose.yml` file in the explorer. This will not work because Visual Studio Code does not include the associated `docker-compose.override.yml` file in the compose operation.

### Debugging with Visual Studio Code

Visual Studio Code supports attaching to containers for debugging. Once the application has started, go to the *Run* tab and click on the *Start Debugging* button with the  *Docker .NET Core Attach (Preview)* profile selected:

![Visual Studio Code debugging](media/vscode-debug.png)

Visual Studio Code will ask you to select a container group. Choose *eshopondapr*.

Next, you can select the container you want to attach to. Select one of the .NET microservice containers. 

Finally, Visual Studio Code will ask you if you want to copy the .NET Core debugger to the container. Select *Yes*.

The debugger is now attached to your container and you can set breakpoints in the code of the selected .NET microservice.

## Run eShopOnDapr in Kubernetes

In the `/deploy/k8s` folder, you find Kubernetes manifest files for running eShopOnDapr in Kubernetes. Obviously, you need an operational Kubernetes cluster to run the app in. The manifest files provided are meant for deployment on your local machine. Therefore you need something like MiniKube or Docker Desktop with Kubernetes enabled on your machine. Follow the instructions of these products to install Kubernetes on your machine.

To start the application, you can use the `start-all.ps1` script in the folder. It uses `kubectl` - the Kubernetes CLI - to apply all the manifests to the Kubernetes cluster.

Different parts of the application are exposed as NodePorts to the local machine. This is convenient for testing the application locally. The following diagram depicts how the application is running in Kubernetes:

![eShopOnDapr running in Kubernetes](media/k8s-nodeport.png)

Use the following URLs for testing the application:

| URL                    | Description                             |
|------------------------|-----------------------------------------|
| http://localhost:30000 | Access the eShopOnDapr SPA front-end    |
| http://localhost:30007 | Access the eShopOnDapr health dashboard |

All access to backend APIs is routed through the API Gateway exposed on port `30050`. The Identity server is exposed on port `30008`. All URLs are configured accordingly in the different manifest files.
