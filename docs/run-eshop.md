# Running eShopOnDapr

## Run eShopOnDapr from the CLI using Docker Compose

The root folder of the repository contains [Docker Compose](https://docs.docker.com/compose/) files to run the solution locally. The `docker-compose.yml` file contains the definition of all the images needed to run eShopOnDapr. The `docker-compose.override.yml` file contains the base configuration for all images of the previous file.

To start eShopOnDapr from the CLI, run the following command from the root folder:

```terminal
docker-compose up
```

First Docker pulls the images. This can take some time to complete. Once the images are available, Docker will start the containers. You should now see the application logs in the terminal:

![Application logging](media/docker-application-output.png)

## Run eShopOnDapr from Visual Studio

Use Visual Studio to get the best F5 debugging experience. To start, open the `eShopOnDapr.sln` solution file in Visual Studio. Below you can see the full `eShopOnDapr.sln` solution opened in Visual Studio:

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

## Run eShopOnDapr on a local Kubernetes cluster using Docker for Desktop

You can run eShopOnDapr on a local Kubernetes cluster by leveraging the [support for Kubernetes in Docker for Desktop](https://docs.docker.com/desktop/kubernetes/).

1. Install the NGINX ingress controller:

   ```terminal
   kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.1.3/deploy/static/provider/cloud/deploy.yaml
   ```

2. Deploy Dapr to the cluster (see the [*Install Dapr into a Kubernetes cluster how-to*](https://docs.dapr.io/getting-started/install-dapr-kubernetes/) for details.):

   ```terminal
   dapr init -k
   ```

3. eShopOnDapr includes a [Helm](https://helm.sh/) chart for easy deployment to a Kubernetes cluster. Run the following command from the `deploy\k8s\helm` folder:

   ```terminal
   helm install --set hostName=kubernetes.docker.internal myeshop .
   ```

   After a little while, you should be able to access the eShopOnDapr health UI at http://kubernetes.docker.internal/status.

   When all microservices are healthy, you can navigate to http://kubernetes.docker.internal to view the eShopOnDapr UI.

To remove eShopOnDapr from Kubernetes, uninstall the Helm chart:

```terminal
helm uninstall myeshop
```

## Run eShopOnDapr on an external Kubernetes cluster

You can run eShopOnDapr on any external Kubernetes cluster, such as Azure Kubernetes Service or Amazon EKS. The following steps describe deploying eShopOnDapr to an AKS cluster:

1. Create a new resource group:

   ```terminal
   az group create --name eShopOnDaprAKS --location westeurope
   ```

2. Create the AKS cluster:

   ```terminal
   az aks create --resource-group eShopOnDaprAKS --name eshopAKSCluster
   ```

3. Configure `kubectl` to connect to the new cluster:

   ```terminal
   az aks get-credentials --resource-group eShopOnDaprAKS --name eshopAKSCluster
   ```

4. Install the NGINX ingress controller:

   ```terminal
   kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.1.3/deploy/static/provider/cloud/deploy.yaml
   ```

5. Deploy Dapr to the cluster (see the [*Install Dapr into a Kubernetes cluster how-to*](https://docs.dapr.io/getting-started/install-dapr-kubernetes/) for details.):

   ```terminal
   dapr init -k
   ```

6. Get the IP address of the cluster load balancer's public endpoint:

   ```terminal
   kubectl get services ingress-nginx-controller -n ingress-nginx -o=jsonpath='{.status.loadBalancer.ingress[0].ip}'
   ```

7. Configuring DNS is outside the scope of these instructions. Instead of using a 'real' hostname, we'll configure a local `eshopondapr.internal` DNS alias that you can use to reach the Kubernetes cluster.

   **Windows**

   On Windows, open the following file: `c:\Windows\System32\Drivers\etc\hosts`

   Add the alias to the bottom of the file and save it. Use the IP address obtained in step 6, for example:

   ```
   20.73.115.88 eshopondapr.internal
   ```

   **macOS**

   On macOS, open the following file: `/private/etc/hosts`

    Add the alias to the bottom of the file and save it. Use the IP address obtained in step 6, for example:

   ```
   20.73.115.88 eshopondapr.internal
   ```

   After saving your changes, you may need to run `dscacheutil -flushcache` to force your changes to have effect.

8. eShopOnDapr includes a [Helm](https://helm.sh/) chart for easy deployment to a Kubernetes cluster. Run the following command from the `deploy\k8s\helm` folder:

   ```terminal
   helm install --set hostName=eshopondapr.internal myeshop .
   ```

   After a little while, you should be able to access the eShopOnDapr health UI at http://eshopondapr.internal/status.

   When all microservices are healthy, you can navigate to http://eshopondapr.internal to view the eShopOnDapr UI.

To remove eShopOnDapr from Kubernetes, uninstall the Helm chart:

```terminal
helm uninstall myeshop
```

## Run eShopOnDapr on Azure Container Apps

eShopOnDapr includes Bicep files for easy deployment to Azure Container Apps. Run the following commands from the `deploy\containerapps` folder to start install using the Azure CLI:

```terminal
az group create --name eShopOnContainerApps --location eastus

az deployment group create --resource-group eShopOnContainerApps --template-file main.bicep
```

Use the following commands to get the URLs to the eShopOnDapr health UI and start pages respectively (requires CLI 2.37.0 or higher):

```terminal
az containerapp show --name webstatus --resource-group eShopOnContainerApps --query "properties.configuration.ingress.fqdn" --output tsv

az containerapp show --name blazor-client --resource-group eShopOnContainerApps --query "properties.configuration.ingress.fqdn" --output tsv
```
