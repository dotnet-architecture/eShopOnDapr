# eShop on Dapr

These are high-level steps:

1. Deploy an AKS cluster with the base application

2. Set up a private registry in ACR

3. Deploy changes

You can run the whole process from the [Azure Cloud Shell](https://docs.microsoft.com/azure/cloud-shell/overview) in bash mode, without any other local resource. You'll need an Azure subscription and you can create a free one in few seconds if you need it.

## Steps

### 1. Deploy an AKS cluster with the base application

- Follow the [quickstart](deploy/k8s/README.md) guide to create an AKS with the base images from "[eshoplearn](https://hub.docker.com/orgs/eshoplearn/repositories)" in DockerHub.

### 2. Set up a private registry in ACR

- Go to directory `deploy/k8s`.
- Run the `./create-acr.sh` script.

### 3. Deploy changes

1. Build images for the applications and push them to the ACR.
   - Go to directory `deploy/k8s`.
   - Run the `./build-to-acr.sh` script.

2. Deploy the services.
   - Go to directory `deploy/k8s`.
   - Run the `./update-aks.sh` script.

A few seconds after running the `update-aks.sh` script you should see something like this in the webstatus page:

![](media/eshoponcontainers-webstatus-failing-services-after-update.png)

And after two or three minutes you should see all services running again:

![](media/eshoponcontainers-webstatus-working-services-after-update.png)