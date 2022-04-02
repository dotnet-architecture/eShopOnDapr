The HTTP application routing solution makes it easy to access applications that are deployed to your Azure Kubernetes Service (AKS) cluster. When the solution's enabled, it configures an Ingress controller in your AKS cluster. As applications are deployed, the solution also creates publicly accessible DNS names for application endpoints.

When the add-on is enabled, it creates a DNS Zone in your subscription. For more information about DNS cost, see DNS pricing.

 Caution

The HTTP application routing add-on is designed to let you quickly create an ingress controller and access your applications. This add-on is not currently designed for use in a production environment and is not recommended for production use. For production-ready ingress deployments that include multiple replicas and TLS support, see Create an HTTPS ingress controller.

Create a resource group:

```bash
az group create --name eShopOnDapr --location eastus
```

Create an AKS cluster:

```bash
az aks create \
    --resource-group eShopOnDapr \
    --name myAKSCluster \
    --node-count 1 \
    --enable-addons http_application_routing,monitoring \
    --generate-ssh-keys
```

Once the cluster is up and running, you'll need to configure your local kubectl tool to access AKS cluster. You can do that by running the following command.

az aks get-credentials --resource-group <resource-group-name> --name <k8s-cluster-name>


```bash
az aks get-credentials --resource-group eShopOnDapr --name myAKSCluster
```

kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.1.1/deploy/static/provider/cloud/deploy.yaml


dapr init -k