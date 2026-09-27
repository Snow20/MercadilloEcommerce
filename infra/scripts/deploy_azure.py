#!/usr/bin/env python3
import os
import subprocess
import json
import sys

def run_cmd(cmd, cwd=None):
    print(f"[AZURE-DEPLOY] Running: {cmd}")
    res = subprocess.run(cmd, shell=True, cwd=cwd, capture_output=True, text=True)
    if res.returncode != 0:
        print(f"[ERROR] {res.stderr}")
        sys.exit(res.returncode)
    print(res.stdout)
    return res.stdout

def main():
    tf_dir = os.path.abspath("../terraform")
    ansible_dir = os.path.abspath("../ansible")

    # 1. Terraform Azure
    run_cmd("terraform init", cwd=tf_dir)
    run_cmd("terraform apply -auto-approve", cwd=tf_dir)
    
    tf_out = json.loads(run_cmd("terraform output -json", cwd=tf_dir))
    rg_name = tf_out["resource_group_name"]["value"]
    aks_name = tf_out["aks_cluster_name"]["value"]

    # 2. Obtener Credenciales de Azure AKS
    run_cmd(f"az aks get-credentials --resource-group {rg_name} --name {aks_name} --overwrite-existing")

    # 3. Disparar Ansible Playbook para configurar clúster
    run_cmd("ansible-playbook -i inventory.azure.yml playbook_azure.yml", cwd=ansible_dir)

if __name__ == "__main__":
    main()