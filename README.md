OneWay Encryptor
================

Android implementation of AsymmetricCrypt (https://github.com/galets/AsymmetricCrypt) file encryption tool

Purpose:
--------

Sometimes, a user of device is going through places, where his device could be taken and examined for
prohibited data. In some cases, the adversary could even decrypt encrypted files, by inspecting memory of
device.

This tool is used to encrypt files on user device in such way that it cannot be decrypted. Device never
gets to know the private key. Therefore, none of the files encrypted using the OneWay application could 
possibly be decrypted using only information on device.


Usage:
------

Use AsymmetricCrypt (https://github.com/galets/AsymmetricCrypt) to generate public/private keypair. Store
private key safely. Upload public key to device using copy/paste or by generating QR code

Files encrypted with OneWay could only be decrypted by AsymmetricCrypt using private key

