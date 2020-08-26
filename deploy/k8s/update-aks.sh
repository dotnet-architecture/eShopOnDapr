#!/bin/bash

# Color theming
if [ -f ~/clouddrive/aspnet-learn/setup/theme.sh ]
then
  . <(cat ~/clouddrive/aspnet-learn/setup/theme.sh)
fi

if [ -f ~/clouddrive/aspnet-learn/create-aks-exports.txt ]
then
  eval $(cat ~/clouddrive/aspnet-learn/create-aks-exports.txt)
fi

if [ -f ~/clouddrive/aspnet-learn/create-acr-exports.txt ]
then
  eval $(cat ~/clouddrive/aspnet-learn/create-acr-exports.txt)
fi

if [ -z "$ESHOP_REGISTRY" ]
then
    echo "ERROR: The ESHOP_REGISTRY environment variable is not defined."
    exit 1
fi

if [ -z "$ESHOP_LBIP" ]
then
    echo "ERROR: The ESHOP_LBIP environment variable is not defined."
    exit 1
fi

echo "Updating existing AKS deployment..."

pushd ~/clouddrive/aspnet-learn/src/deploy/k8s

# Uninstall charts to be updated
for chart in webspa webstatus webshoppingagg
do
    echo
    echo "Uninstalling chart \"$chart\"..."
    echo "${newline}${genericCommandStyle}helm uninstall eshop-$chart${defaultTextStyle}${newline}"
    helm uninstall eshop-$chart
done

# Install reconfigured charts from Docker Hub
for chart in webstatus webshoppingagg
do
    echo
    echo "Installing chart \"$chart\"..."
    echo "${newline}${genericCommandStyle}helm install eshop-$chart --set registry=eshoplearn --set aksLB=$ESHOP_LBIP \"helm-simple/$chart\"${defaultTextStyle}${newline}"
    helm install eshop-$chart --set registry=eshoplearn --set aksLB=$ESHOP_LBIP "helm-simple/$chart"
done

# Install charts for new and updated applications from ACR
for chart in coupon webspa 
do
    echo
    echo "Installing chart \"$chart\"..."
    echo "${newline}${genericCommandStyle}helm install eshop-$chart --set registry=$ESHOP_REGISTRY --set aksLB=$ESHOP_LBIP \"helm-simple/$chart\"${defaultTextStyle}${newline}"
    helm install eshop-$chart --set registry=$ESHOP_REGISTRY --set aksLB=$ESHOP_LBIP "helm-simple/$chart"
done

popd

echo "Done updating existing AKS deployment!"
