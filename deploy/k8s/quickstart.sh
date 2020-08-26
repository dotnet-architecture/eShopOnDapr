#!/bin/bash

eshopSubs=${ESHOP_SUBS}
eshopRg=${ESHOP_RG}
eshopLocation=${ESHOP_LOCATION}
eshopRegistry=eshoplearn

while [ "$1" != "" ]; do
    case $1 in
        -s | --subscription)            shift
                                        eshopSubs=$1
                                        ;;
        -g | --resource-group)          shift
                                        eshopRg=$1
                                        ;;
        -l | --location)                shift
                                        eshopLocation=$1
                                        ;;
             * )                        echo "Invalid param: $1"
                                        exit 1
    esac
    shift
done

if [ -z "$eshopRg" ]
then
    echo "${newline}${errorStyle}ERROR: Resource group is mandatory. Use -g to set it.${defaultTextStyle}${newline}"
    exit 1
fi

if [ ! -z "$eshopSubs" ]
then
    echo "Switching to subscription $eshopSubs..."
    az account set -s $eshopSubs
fi

if [ ! $? -eq 0 ]
then
    echo "${newline}${errorStyle}ERROR: Can't switch to subscription $eshopSubs.${defaultTextStyle}${newline}"
    exit 1
fi

export ESHOP_SUBS=$eshopSubs
export ESHOP_RG=$eshopRg
export ESHOP_LOCATION=$eshopLocation
export ESHOP_REGISTRY=$eshopRegistry
export ESHOP_QUICKSTART=true

cd ~/clouddrive/aspnet-learn/src/deploy/k8s

# AKS Cluster creation

./create-aks.sh

eval $(cat ~/clouddrive/aspnet-learn/create-aks-exports.txt)

./deploy-aks.sh
