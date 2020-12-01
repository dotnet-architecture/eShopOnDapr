# eShop on Dapr

A sample .NET Core distributed application based on *[eShopOnContainers](https://github.com/dotnet-architecture/eShopOnContainers)*, powered by [Dapr](https://dapr.io/).

![eShopOnDapr](docs/media/eshopondapr.png)

Dapr enables developers using any language or framework to easily write microservices. It addresses many of the challenges found that come along with distributed applications, such as:

- How can distributed services discover each other and communicate synchronously?
- How can they implement asynchronous messaging? 
- How can they maintain contextual information across a transaction?
- How can they become resilient to failure?
- How can they scale to meet fluctuating demand?
- How are they monitored and observed?

> The code in this repository is **work in progress**. An accompanying e-Book called *Dapr for .NET developers* is currently in development and will use the sample code in this repository to demonstrate Dapr features and benefits.

## Getting started

eShopOnDapr runs in containers and requires Docker to run. There are various ways to start the application:

- [Run eShopOnDapr from the CLI](docs/run-eshop.md#run-eshopondapr-from-the-cli)
- [Run eShopOnDapr from Visual Studio (best F5 debugging experience)](docs/run-eshop.md#run-eshopondapr-from-visual-studio)
- [Run eShopOnDapr from Visual Studio Code (allows you to debug individual containers))](docs/run-eshop.md#run-eshopondapr-from-visual-studio-code)

> Support for Kubernetes deployments is on the roadmap.

Note that it will take a little while to start all containers. eShopOnDapr includes a health UI that you can use to see if the containers are ready. You can access it at `http://localhost:5107/hc-ui`.

When all microservices are healthy, you can navigate to http://localhost:5104 to view the eShopOnDapr UI.

## Roadmap

- [ ] Deployment
  - [x] Standalone: Docker Compose
  - [ ] K8s
- [x] Service invocation
- [x] Pub/sub
- [x] State management
- [ ] Secrets
  - [x] in configuration files
  - [ ] in .NET startup code
- [x] Observability
- [ ] Actor model
- [x] Bindings

### Attributions

Model photos by  [Laura Chouette](https://unsplash.com/@laurachouette?utm_source=unsplash&utm_medium=referral&utm_content=creditCopyText), [Heng Films](https://unsplash.com/@hengfilms?utm_source=unsplash&utm_medium=referral&utm_content=creditCopyText) & [Enmanuel betances Santos](https://unsplash.com/@enmanuelbs?utm_source=unsplash&utm_medium=referral&utm_content=creditCopyText) on  [Unsplash](https://unsplash.com/photos/HqtYwlY9dxs?utm_source=unsplash&utm_medium=referral&utm_content=creditCopyText).
