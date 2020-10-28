kubectl delete `
    -f ./catalog.yaml `
    -f ./ordering.yaml `
    -f ./component-pubsub.yaml `
    -f ./component-sendmail.yaml `
    -f ./component-zipkin.yaml `
    -f ./seq.yaml `
    -f ./sqldata.yaml `
    -f ./nats-streaming.yaml