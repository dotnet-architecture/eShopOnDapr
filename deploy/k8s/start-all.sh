#!/bin/bash

kubectl apply \
    -f ./namespace.yaml \
    -f ./dapr-config.yaml \
    -f ./secrets.yaml \
    -f ./seq.yaml \
    -f ./zipkin.yaml \
    -f ./sqldata.yaml \
    -f ./redis.yaml \
    -f ./signalr.yaml \
    -f ./identity.yaml \
    -f ./apigateway.yaml \
    -f ./components/pubsub-redis.yaml \
    -f ./components/basket-statestore.yaml \
    -f ./components/sendmail.yaml \
    -f ./catalog.yaml \
    -f ./ordering.yaml \
    -f ./basket.yaml \
    -f ./payment.yaml \
    -f ./backgroundtasks.yaml \
    -f ./webshoppingagg.yaml \
    -f ./webspa.yaml \
    -f ./webstatus.yaml
