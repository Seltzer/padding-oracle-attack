# padding-oracle-attack

Playing around with padding oracle attacks in .NET. 

Oracles:
- One which executes the crypto directly and waits for CryptographicExceptions
- One which executes the crypto directly and measures timings

Cryptos:
- One which uses Rijndael.Create()
- One which uses Aes.Create()