.\cfssl.exe gencert -initca csr-ca.json | .\cfssljson.exe -bare ca
.\cfssl.exe gencert -ca ca.pem -ca-key ca-key.pem -config config.json csr-server.json | .\cfssljson.exe -bare srv
cat srv.pem, ca.pem > chain.pem