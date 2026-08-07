#!/usr/bin/env python3
"""
Script para baixar o Swagger da API, gerar uma Postman Collection
e importar automaticamente no workspace "FinancialHub" do Postman.
Uso: python export-swagger.py [URL] [POSTMAN_API_KEY]
"""

import json
import os
import ssl
import sys
import urllib.request

# A chave é lida de um arquivo local fora do repositório (nunca commitada).
# Alternativas: variável de ambiente POSTMAN_API_KEY ou 2º argumento na linha de comando.
POSTMAN_API_KEY_FILE = r"C:\workarea\chaves\Postman\POSTMAN_API_KEY.txt"
POSTMAN_WORKSPACE_NAME = "FinancialHub"


def load_postman_api_key():
    if os.path.exists(POSTMAN_API_KEY_FILE):
        with open(POSTMAN_API_KEY_FILE, "r", encoding="utf-8") as f:
            return f.read().strip()
    return os.environ.get("POSTMAN_API_KEY", "")


POSTMAN_API_KEY = load_postman_api_key()


def generate_example(schema, swagger, depth=0):
    """Gera um valor de exemplo a partir de um schema OpenAPI"""
    if depth > 5:
        return None

    if "$ref" in schema:
        ref_path = schema["$ref"].split("/")
        if ref_path[0] == "#" and ref_path[1] == "components" and ref_path[2] == "schemas":
            component_name = ref_path[3]
            component_schema = swagger.get("components", {}).get("schemas", {}).get(component_name, {})
            return generate_example(component_schema, swagger, depth + 1)

    if "example" in schema:
        return schema["example"]

    schema_type = schema.get("type", "object")

    if schema_type == "object":
        obj = {}
        properties = schema.get("properties", {})
        required = schema.get("required", [])
        for prop_name, prop_schema in properties.items():
            if prop_name in required or len(required) == 0:
                obj[prop_name] = generate_example(prop_schema, swagger, depth + 1)
        return obj

    if schema_type == "array":
        items_schema = schema.get("items", {})
        return [generate_example(items_schema, swagger, depth + 1)]

    if schema_type == "string":
        format_type = schema.get("format", "")
        if format_type == "uuid":
            return "00000000-0000-0000-0000-000000000000"
        if format_type == "date-time":
            return "2024-01-01T00:00:00Z"
        if format_type == "date":
            return "2024-01-01"
        if format_type == "email":
            return "email@example.com"
        if schema.get("enum"):
            return schema["enum"][0]
        return "string"

    if schema_type == "integer":
        return 0

    if schema_type == "number":
        return 0.0

    if schema_type == "boolean":
        return False

    return None


def build_request(path, method, endpoint, swagger):
    """Monta o request do Postman para um endpoint do swagger"""
    request = {
        "name": endpoint.get("summary", f"{method.upper()} {path}"),
        "request": {
            "method": method.upper(),
            "header": [],
            "url": {
                "raw": "{{host}}" + path,
                "host": ["{{host}}"],
                "path": [p for p in path.split("/") if p],
                "query": []
            }
        }
    }

    for param in endpoint.get("parameters", []):
        if param.get("in") == "query":
            value = param.get("default", param.get("example", ""))
            request["request"]["url"]["query"].append({
                "key": param.get("name"),
                "value": str(value),
                "description": param.get("description", "")
            })

    if method in ["post", "put", "patch"] and endpoint.get("requestBody"):
        content_types = endpoint["requestBody"].get("content", {})
        schema = content_types.get("application/json", {}).get("schema")
        if schema:
            request["request"]["header"].append({"key": "Content-Type", "value": "application/json"})
            body_example = json.dumps(generate_example(schema, swagger), indent=2, ensure_ascii=False)
            request["request"]["body"] = {
                "mode": "raw",
                "raw": body_example,
                "options": {"raw": {"language": "json"}}
            }

    return request


def get_postman_workspaces():
    """Lista todos os workspaces do Postman"""
    try:
        req = urllib.request.Request(
            "https://api.getpostman.com/workspaces",
            headers={"X-Api-Key": POSTMAN_API_KEY}
        )
        with urllib.request.urlopen(req) as response:
            return json.loads(response.read().decode())
    except Exception as e:
        print(f"\033[31m✗ Erro ao listar workspaces: {e}\033[0m")
        return None


def get_postman_collections(workspace_id=None):
    """Lista todas as coleções do Postman"""
    try:
        url = "https://api.getpostman.com/collections"
        if workspace_id:
            url = f"{url}?workspace={workspace_id}"
        req = urllib.request.Request(
            url,
            headers={"X-Api-Key": POSTMAN_API_KEY}
        )
        with urllib.request.urlopen(req) as response:
            return json.loads(response.read().decode())
    except Exception as e:
        print(f"\033[31m✗ Erro ao listar coleções: {e}\033[0m")
        return None


def delete_postman_collection(collection_uid):
    """Deleta uma coleção do Postman"""
    try:
        req = urllib.request.Request(
            f"https://api.getpostman.com/collections/{collection_uid}",
            method="DELETE",
            headers={"X-Api-Key": POSTMAN_API_KEY}
        )
        with urllib.request.urlopen(req) as response:
            return response.status == 200
    except Exception as e:
        print(f"\033[31m✗ Erro ao deletar coleção: {e}\033[0m")
        return False


def import_postman_collection(collection_json, workspace_id=None):
    """Importa uma coleção para o Postman"""
    try:
        url = "https://api.getpostman.com/collections"
        if workspace_id:
            url = f"{url}?workspace={workspace_id}"

        data = json.dumps({"collection": collection_json}).encode("utf-8")
        req = urllib.request.Request(
            url,
            data=data,
            method="POST",
            headers={
                "X-Api-Key": POSTMAN_API_KEY,
                "Content-Type": "application/json"
            }
        )
        with urllib.request.urlopen(req) as response:
            return json.loads(response.read().decode())
    except Exception as e:
        print(f"\033[31m✗ Erro ao importar coleção: {e}\033[0m")
        return None


def sync_to_postman(collection):
    """Remove a coleção existente no workspace e importa a versão atual"""
    print()
    print("\033[36m🔄 Atualizando Postman...\033[0m")

    workspace_id = None
    workspaces = get_postman_workspaces()
    if workspaces:
        for ws in workspaces.get("workspaces", []):
            if ws.get("name") == POSTMAN_WORKSPACE_NAME:
                workspace_id = ws.get("id")
                print(f"\033[32m✓ Workspace encontrado: {ws.get('name')} (ID: {workspace_id})\033[0m")
                break

    if not workspace_id:
        print(f"\033[33m⚠️  Workspace '{POSTMAN_WORKSPACE_NAME}' não encontrado. Usando workspace padrão.\033[0m")

    collections = get_postman_collections(workspace_id)
    if not collections:
        return

    collection_name = collection["info"]["name"]
    existing = next(
        (col for col in collections.get("collections", []) if col.get("name") == collection_name),
        None
    )

    if existing:
        print(f"\033[33m🗑️  Removendo coleção existente: {existing['name']}\033[0m")
        if delete_postman_collection(existing["uid"]):
            print("\033[32m✓ Coleção removida\033[0m")
        else:
            print("\033[31m✗ Falha ao remover coleção\033[0m")

    print("\033[36m📤 Importando nova coleção...\033[0m")
    result = import_postman_collection(collection, workspace_id)
    if result and result.get("collection"):
        print("\033[32m✓ Coleção importada com sucesso!\033[0m")
        print(f"\033[37m   ID: {result['collection'].get('uid')}\033[0m")
        print(f"\033[37m   Workspace: {POSTMAN_WORKSPACE_NAME}\033[0m")
    else:
        print("\033[31m✗ Falha ao importar coleção\033[0m")


def main():
    global POSTMAN_API_KEY

    url = sys.argv[1] if len(sys.argv) > 1 else "https://localhost:7206"
    if len(sys.argv) > 2:
        POSTMAN_API_KEY = sys.argv[2]

    swagger_url = f"{url}/swagger/v1/swagger.json"
    output_file = "postman-collection.json"

    print(f"\033[36m📥 Baixando Swagger de: {swagger_url}\033[0m")

    try:
        context = ssl._create_unverified_context()
        with urllib.request.urlopen(swagger_url, context=context) as response:
            swagger = json.loads(response.read().decode())
        print("\033[32m✓ Swagger baixado\033[0m")
    except Exception as e:
        print(f"\033[31m✗ Erro ao baixar: {e}\033[0m")
        print(f"\033[33mCertifique-se de que a API está rodando em: {url}\033[0m")
        sys.exit(1)

    print("\033[36m🔄 Convertendo para Postman Collection...\033[0m")

    collection = {
        "info": {
            "name": swagger.get("info", {}).get("title", "API Collection"),
            "description": swagger.get("info", {}).get("description", ""),
            "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
        },
        "item": [],
        "variable": [
            {"key": "host", "value": url, "type": "string"}
        ]
    }

    # Agrupar endpoints por tag (feature)
    folders = {}
    for path, methods in swagger.get("paths", {}).items():
        for method, endpoint in methods.items():
            if method not in ["get", "post", "put", "patch", "delete"]:
                continue

            tag = endpoint.get("tags", ["Other"])[0]
            folders.setdefault(tag, {"name": tag, "item": []})
            folders[tag]["item"].append(build_request(path, method, endpoint, swagger))

    collection["item"] = list(folders.values())

    if os.path.exists(output_file):
        os.remove(output_file)
        print("\033[33m🗑️  Arquivo existente removido\033[0m")

    with open(output_file, "w", encoding="utf-8") as f:
        json.dump(collection, f, indent=2, ensure_ascii=False)

    print(f"\033[32m✓ Collection criada: {output_file}\033[0m")

    if POSTMAN_API_KEY:
        sync_to_postman(collection)
    else:
        print()
        print("\033[36mPara importar no Postman:\033[0m")
        print("\033[37m1. Abra o Postman\033[0m")
        print("\033[37m2. Import → Selecione o arquivo\033[0m")
        print("\033[37m3. A variável 'host' já vem preenchida com a URL usada\033[0m")
        print()
        print(f"\033[33m💡 Dica: crie o arquivo {POSTMAN_API_KEY_FILE} com a chave")
        print("   (ou defina POSTMAN_API_KEY, ou passe como 2º argumento) para importar")
        print("   automaticamente no workspace FinancialHub\033[0m")


if __name__ == "__main__":
    main()
