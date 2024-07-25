import sys
import requests
import json
import os

# Retrieve command line arguments
prompt_text = sys.argv[1]  # First argument: prompt

# OpenAI API Key
api_key = os.getenv('OPENAI_KEY')
if not api_key:
    raise ValueError("No API key provided. Set the OPENAI_KEY environment variable.")

# Set up headers and payload
headers = {
    "Content-Type": "application/json",
    "Authorization": f"Bearer {api_key}"
}

payload = {
    "model": "gpt-4o",
    "messages": [
        {
            "role": "user",
            "content": prompt_text
        }
    ],
    "max_tokens": 300
}

# Send request to OpenAI API
response = requests.post("https://api.openai.com/v1/chat/completions", headers=headers, json=payload)

# Print the response to stdout (to be captured by Unity)
print(json.dumps(response.json()))
