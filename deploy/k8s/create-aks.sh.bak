#!/bin/bash

# Color theming
if [ -f ~/clouddrive/aspnet-learn/setup/theme.sh ]
then
  . <(cat ~/clouddrive/aspnet-learn/setup/theme.sh)
fi

eshopSubs=${ESHOP_SUBS}
eshopRg=${ESHOP_RG}
eshopLocation=${ESHOP_LOCATION}
eshopNodeCount=${ESHOP_NODECOUNT:-1}
eshopRegistry=${ESHOP_REGISTRY}
eshopAcrName=${ESHOP_ACRNAME}
eshopClientId=${ESHOP_CLIENTID}
eshopClientSecret=${ESHOP_CLIENTSECRET}

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
             --acr-name)                shift
                                        eshopAcrName=$1
                                        ;;
             --appid)                   shift
                                        eshopClientId=$1
                                        ;;
             --password)                shift
                                        eshopClientSecret=$1
                                        ;;
             * )                        echo "Invalid param: $1"
                                        exit 1
    esac
    shift
done

if [ -z "$eshopRg" ]
then
    echo "${newline}${errorStyle}ERROR: resource group is mandatory. Use -g to set it.${defaultTextStyle}${newline}"
    exit 1
fi

if [ -z "$eshopAcrName" ]&&[ -z "$ESHOP_QUICKSTART" ]
then
    echo "${newline}${errorStyle}ERROR: ACR name is mandatory. Use --acr-name to set it.${defaultTextStyle}${newline}"
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

# Swallow STDERR so we don't get red text here from expected error if the RG doesn't exist
exec 3>&2
exec 2> /dev/null

rg=`az group show -g $eshopRg -o json`

# Reset STDERR
exec 2>&3

if [ -z "$rg" ]
then
    if [ -z "eshopSubs" ]
    then
        echo "${newline}${errorStyle}ERROR: If resource group has to be created, location is mandatory. Use -l to set it.${defaultTextStyle}${newline}"
        exit 1
    fi
    echo "Creating resource group $eshopRg in location $eshopLocation..."
    echo "${newline} > ${azCliCommandStyle}az group create -n $eshopRg -l $eshopLocation --output none${defaultTextStyle}${newline}"
    az group create -n $eshopRg -l $eshopLocation --output none
    if [ ! $? -eq 0 ]
    then
        echo "${newline}${errorStyle}ERROR: Can't create resource group!${defaultTextStyle}${newline}"
        exit 1
    fi
else
    if [ -z "$eshopLocation" ]
    then
        eshopLocation=`az group show -g $eshopRg --query "location" -otsv`
    fi
fi

# Service Principal creation / validation

if [ -z "$eshopClientId" ] || [ -z "$eshopClientSecret" ]
then
    echo "Creating service principal..."

    spHomepage="https://eShop-Learn-AKS-SP"$RANDOM
    eshopClientAppCommand="az ad sp create-for-rbac --name "$spHomepage" --query "[appId,password]" -otsv"

    echo "${newline} > ${azCliCommandStyle}$eshopClientAppCommand${defaultTextStyle}${newline}"
    eshopClientApp=`$eshopClientAppCommand`
    
    if [ ! $? -eq 0 ]
    then
        echo "${newline}${errorStyle}ERROR: Can't create service principal for AKS.${defaultTextStyle}${newline}"
        exit 1
    fi

    eshopClientId=`echo "$eshopClientApp" | head -1`
    eshopClientSecret=`echo "$eshopClientApp" | tail -1`

    if [ "$eshopClientId" == "" ]||[ "$eshopClientSecret" == "" ]
    then
        echo "${newline}${errorStyle}ERROR: ClientId (\"$eshopClientId\") or ClientSecret (\"$eshopClientSecret\") missing!${defaultTextStyle}${newline}"
        exit 1
    fi

    echo
    echo "Service principal \"$spHomepage\" created with ID \"$eshopClientId\" and password \"$eshopClientSecret\""
fi

# AKS Cluster creation

eshopAksName="eshop-learn-aks"

echo
echo "Creating AKS cluster \"$eshopAksName\" in resource group \"$eshopRg\" and location \"$eshopLocation\"..."
aksCreateCommand="az aks create -n $eshopAksName -g $eshopRg --node-count $eshopNodeCount --node-vm-size Standard_D2_v3 --vm-set-type VirtualMachineScaleSets -l $eshopLocation --client-secret $eshopClientSecret --service-principal $eshopClientId --generate-ssh-keys -o json"
echo "${newline} > ${azCliCommandStyle}$aksCreateCommand${defaultTextStyle}${newline}"
retry=5
aks=`$aksCreateCommand`
while [ ! $? -eq 0 ]&&[ $retry -gt 0 ]&&[ ! -z "$spHomepage" ]
do
    echo
    echo "New service principal is not yet ready for AKS cluster creation. ${bold}This is normal and expected.${defaultTextStyle} Retrying in 5s..."
    let retry--
    sleep 5
    echo
    echo "Retrying AKS cluster creation..."
    aks=`$aksCreateCommand`
done

if [ ! $? -eq 0 ]
then
    echo "${newline}${errorStyle}Error creating AKS cluster!${defaultTextStyle}${newline}"
    exit 1
fi

echo
echo "AKS cluster created."

if [ ! -z "$eshopAcrName" ]
then
    echo
    echo "Granting AKS pull permissions from ACR $eshopAcrName"
    az aks update -n $eshopAksName -g $eshopRg --attach-acr $eshopAcrName
fi

echo
echo "Getting credentials for AKS..."
az aks get-credentials -n $eshopAksName -g $eshopRg --overwrite-existing

# Ingress controller and load balancer (LB) deployment

echo
echo "Installing NGINX ingress controller"
kubectl apply -f ingress-controller/nginx-mandatory.yaml
kubectl apply -f ingress-controller/nginx-service-loadbalancer.yaml
kubectl apply -f ingress-controller/nginx-cm.yaml

echo
echo "Getting load balancer public IP"

k8sLbTag="ingress-nginx/ingress-nginx"
aksNodeRGCommand="az aks list --query \"[?name=='$eshopAksName'&&resourceGroup=='$eshopRg'].nodeResourceGroup\" -otsv"

retry=5
echo "${newline} > ${azCliCommandStyle}$aksNodeRGCommand${defaultTextStyle}${newline}"
aksNodeRG=$(eval $aksNodeRGCommand)
while [ "$aksNodeRG" == "" ]
do
    echo
    echo "Unable to obtain load balancer resource group. Retrying in 5s..."
    let retry--
    sleep 5
    echo
    echo "Retrying..."
    echo $aksNodeRGCommand
    aksNodeRG=$(eval $aksNodeRGCommand)
done


while [ "$eshopLbIp" == "" ]
do
    eshopLbIpCommand="az network public-ip list -g $aksNodeRG --query \"[?tags.service=='$k8sLbTag'].ipAddress\" -otsv"
    echo "${newline} > ${azCliCommandStyle}$eshopLbIpCommand${defaultTextStyle}${newline}"
    eshopLbIp=$(eval $eshopLbIpCommand)
    echo "Waiting for the load balancer IP address..."
    sleep 5
done

echo "Done!"

echo export ESHOP_SUBS=$eshopSubs > create-aks-exports.txt
echo export ESHOP_RG=$eshopRg >> create-aks-exports.txt
echo export ESHOP_LOCATION=$eshopLocation >> create-aks-exports.txt

if [ ! -z "$eshopAcrName" ]
then
    echo export ESHOP_ACRNAME=$eshopAcrName >> create-aks-exports.txt
fi

if [ ! -z "$eshopRegistry" ]
then
    echo export ESHOP_REGISTRY=$eshopRegistry >> create-aks-exports.txt
fi

if [ "$spHomepage" != "" ]
then
    echo export ESHOP_CLIENTID=$eshopClientId >> create-aks-exports.txt
    echo export ESHOP_CLIENTPASSWORD=$eshopClientSecret >> create-aks-exports.txt
fi

echo export ESHOP_LBIP=$eshopLbIp >> create-aks-exports.txt

if [ -z "$ESHOP_QUICKSTART" ]
then
    echo "Run the following command to update the environment"
    echo 'eval $(cat ~/clouddrive/aspnet-learn/create-aks-exports.txt)'
    echo
fi

mv -f create-aks-exports.txt ~/clouddrive/aspnet-learn/
