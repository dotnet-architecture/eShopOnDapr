FROM envoyproxy/envoy:v1.14.2

COPY src/ApiGateways/Envoy/envoy.yaml /tmpl/envoy.yaml.tmpl
COPY src/ApiGateways/Envoy/docker-entrypoint.sh /

RUN chmod 500 /docker-entrypoint.sh

RUN apt-get update && \
    apt-get install gettext -y

ENTRYPOINT ["/docker-entrypoint.sh"]