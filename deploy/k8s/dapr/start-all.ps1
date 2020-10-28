kubectl apply `
    -f ./namespace.yaml `
    -f ./sqldata.yaml `
    -f ./nats-streaming.yaml `
    -f ./seq.yaml `
    -f ./component-pubsub.yaml `
    -f ./component-sendmail.yaml `
    -f ./component-zipkin.yaml `
    -f ./catalog.yaml `
    -f ./ordering.yaml