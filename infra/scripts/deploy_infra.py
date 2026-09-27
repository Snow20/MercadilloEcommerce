#!/usr/bin/env python3
import os
import subprocess
import json
import sys

def run_command(command, cwd=None):
    print(f"[EXEC] {command}")
    result = subprocess.run(command, shell=True, cwd=cwd, capture_output=True, text=True)
    if result.returncode != 0:
        print(f"[ERROR] {result.stderr}")
        sys.exit(result.returncode)
    print(result.stdout)
    return result.stdout

def main():
    terraform_dir = os.path.abspath("../terraform")
    ansible_dir = os.path.abspath("../ansible")
    
    print("=== Step 1: Initializing Terraform ===")
    run_command("terraform init", cwd=terraform_dir)
    
    print("=== Step 2: Applying Terraform Infrastructure ===")
    run_command("terraform apply -auto-approve", cwd=terraform_dir)
    
    print("=== Step 3: Fetching Outputs ===")
    output_raw = run_command("terraform output -json", cwd=terraform_dir)
    tf_outputs = json.loads(output_raw)
    server_ip = tf_outputs.get("k8s_node_public_ip", {}).get("value", "127.0.0.1")
    
    print(f"=== Step 4: Generating Ansible Inventory for {server_ip} ===")
    inventory_content = f"""[k8s_nodes]
node1 ansible_host={server_ip} ansible_user=ubuntu ansible_ssh_private_key_file=~/.ssh/id_rsa
"""
    with open(os.path.join(ansible_dir, "hosts.ini"), "w") as f:
        f.write(inventory_content)
        
    print("=== Step 5: Executing Ansible Playbook ===")
    run_command("ansible-playbook -i hosts.ini playbook.yml", cwd=ansible_dir)
    
    print("=== Deployment Completed Successfully ===")

if __name__ == "__main__":
    main()