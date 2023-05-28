# padding-oracle-attack

Playing around with padding oracle attacks in .NET. 

Oracles:
- One which executes the crypto directly and makes decisions based on different types of CryptographicExceptions
- One which executes the crypto directly and makes decisions based on measured timings

Cryptos:
- One which uses Rijndael.Create()
- One which uses Aes.Create()
