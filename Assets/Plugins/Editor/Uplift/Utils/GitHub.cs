// --- BEGIN LICENSE BLOCK ---
/*
 * Copyright (c) 2017-present WeWantToKnow AS
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
// --- END LICENSE BLOCK ---

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using Uplift.Common;
using Uplift.MiniJSON;

namespace Uplift.GitHubModule
{
    public class GitHub
    {
        private static readonly string[] githubCertificatesString = new string[]
        {
// github.com
@"-----BEGIN CERTIFICATE-----
MIIHeTCCBmGgAwIBAgIQC/20CQrXteZAwwsWyVKaJzANBgkqhkiG9w0BAQsFADB1
MQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3
d3cuZGlnaWNlcnQuY29tMTQwMgYDVQQDEytEaWdpQ2VydCBTSEEyIEV4dGVuZGVk
IFZhbGlkYXRpb24gU2VydmVyIENBMB4XDTE2MDMxMDAwMDAwMFoXDTE4MDUxNzEy
MDAwMFowgf0xHTAbBgNVBA8MFFByaXZhdGUgT3JnYW5pemF0aW9uMRMwEQYLKwYB
BAGCNzwCAQMTAlVTMRkwFwYLKwYBBAGCNzwCAQITCERlbGF3YXJlMRAwDgYDVQQF
Ewc1MTU3NTUwMSQwIgYDVQQJExs4OCBDb2xpbiBQIEtlbGx5LCBKciBTdHJlZXQx
DjAMBgNVBBETBTk0MTA3MQswCQYDVQQGEwJVUzETMBEGA1UECBMKQ2FsaWZvcm5p
YTEWMBQGA1UEBxMNU2FuIEZyYW5jaXNjbzEVMBMGA1UEChMMR2l0SHViLCBJbmMu
MRMwEQYDVQQDEwpnaXRodWIuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIB
CgKCAQEA54hc8pZclxgcupjiA/F/OZGRwm/ZlucoQGTNTKmBEgNsrn/mxhngWmPw
bAvUaLP//T79Jc+1WXMpxMiz9PK6yZRRFuIo0d2bx423NA6hOL2RTtbnfs+y0PFS
/YTpQSelTuq+Fuwts5v6aAweNyMcYD0HBybkkdosFoDccBNzJ92Ac8I5EVDUc3Or
/4jSyZwzxu9kdmBlBzeHMvsqdH8SX9mNahXtXxRpwZnBiUjw36PgN+s9GLWGrafd
02T0ux9Yzd5ezkMxukqEAQ7AKIIijvaWPAJbK/52XLhIy2vpGNylyni/DQD18bBP
T+ZG1uv0QQP9LuY/joO+FKDOTler4wIDAQABo4IDejCCA3YwHwYDVR0jBBgwFoAU
PdNQpdagre7zSmAKZdMh1Pj41g8wHQYDVR0OBBYEFIhcSGcZzKB2WS0RecO+oqyH
IidbMCUGA1UdEQQeMByCCmdpdGh1Yi5jb22CDnd3dy5naXRodWIuY29tMA4GA1Ud
DwEB/wQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDAQYIKwYBBQUHAwIwdQYDVR0f
BG4wbDA0oDKgMIYuaHR0cDovL2NybDMuZGlnaWNlcnQuY29tL3NoYTItZXYtc2Vy
dmVyLWcxLmNybDA0oDKgMIYuaHR0cDovL2NybDQuZGlnaWNlcnQuY29tL3NoYTIt
ZXYtc2VydmVyLWcxLmNybDBLBgNVHSAERDBCMDcGCWCGSAGG/WwCATAqMCgGCCsG
AQUFBwIBFhxodHRwczovL3d3dy5kaWdpY2VydC5jb20vQ1BTMAcGBWeBDAEBMIGI
BggrBgEFBQcBAQR8MHowJAYIKwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmRpZ2ljZXJ0
LmNvbTBSBggrBgEFBQcwAoZGaHR0cDovL2NhY2VydHMuZGlnaWNlcnQuY29tL0Rp
Z2lDZXJ0U0hBMkV4dGVuZGVkVmFsaWRhdGlvblNlcnZlckNBLmNydDAMBgNVHRMB
Af8EAjAAMIIBfwYKKwYBBAHWeQIEAgSCAW8EggFrAWkAdgCkuQmQtBhYFIe7E6LM
Z3AKPDWYBPkb37jjd80OyA3cEAAAAVNhieoeAAAEAwBHMEUCIQCHHSEY/ROK2/sO
ljbKaNEcKWz6BxHJNPOtjSyuVnSn4QIgJ6RqvYbSX1vKLeX7vpnOfCAfS2Y8lB5R
NMwk6us2QiAAdgBo9pj4H2SCvjqM7rkoHUz8cVFdZ5PURNEKZ6y7T0/7xAAAAVNh
iennAAAEAwBHMEUCIQDZpd5S+3to8k7lcDeWBhiJASiYTk2rNAT26lVaM3xhWwIg
NUqrkIODZpRg+khhp8ag65B8mu0p4JUAmkRDbiYnRvYAdwBWFAaaL9fC7NP14b1E
sj7HRna5vJkRXMDvlJhV1onQ3QAAAVNhieqZAAAEAwBIMEYCIQDnm3WStlvE99GC
izSx+UGtGmQk2WTokoPgo1hfiv8zIAIhAPrYeXrBgseA9jUWWoB4IvmcZtshjXso
nT8MIG1u1zF8MA0GCSqGSIb3DQEBCwUAA4IBAQCLbNtkxuspqycq8h1EpbmAX0wM
5DoW7hM/FVdz4LJ3Kmftyk1yd8j/PSxRrAQN2Mr/frKeK8NE1cMji32mJbBqpWtK
/+wC+avPplBUbNpzP53cuTMF/QssxItPGNP5/OT9Aj1BxA/NofWZKh4ufV7cz3pY
RDS4BF+EEFQ4l5GY+yp4WJA/xSvYsTHWeWxRD1/nl62/Rd9FN2NkacRVozCxRVle
FrBHTFxqIP6kDnxiLElBrZngtY07ietaYZVLQN/ETyqLQftsf8TecwTklbjvm8NT
JqbaIVifYwqwNN+4lRxS3F5lNlA/il12IOgbRioLI62o8G0DaEUQgHNf8vSG
-----END CERTIFICATE-----
",
// api.github.com
@"-----BEGIN CERTIFICATE-----
MIIHTTCCBjWgAwIBAgIQDZ3d58+sYZrDhm+uNUWKlDANBgkqhkiG9w0BAQsFADBw
MQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3
d3cuZGlnaWNlcnQuY29tMS8wLQYDVQQDEyZEaWdpQ2VydCBTSEEyIEhpZ2ggQXNz
dXJhbmNlIFNlcnZlciBDQTAeFw0xNzAxMTgwMDAwMDBaFw0yMDA0MTcxMjAwMDBa
MGgxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpDYWxpZm9ybmlhMRYwFAYDVQQHEw1T
YW4gRnJhbmNpc2NvMRUwEwYDVQQKEwxHaXRIdWIsIEluYy4xFTATBgNVBAMMDCou
Z2l0aHViLmNvbTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKnAjzBJ
LbsS6AwMXR0ICPeRcHh+BWuvi7JVNqpplARfqbuGTlL6SEMVVOcKnVmsWWrsRtZ2
FE6wFnT29Z9LpoC7BhO1mFyZ0DyXsCCuEIbmtC7K4qyHR5HMB0PNzRGI/pbMIYNH
1EFEbdOlLWuWpC6Lw3STy6k7k0v57ITmu+oUdKLlp66rnCy9bM3L1/6GwfjbG5q+
fDK4PKa0H0aCuom85WcrFfPKj3AqXOe5aukASkNhfVoEDLLEIoF30zZvJQAEPi+o
AmZ4HeCnx2D0iWANO6BVprkjmpYIdNFofD1I7kRq3DcVqZoHwC0BxxWIKA3A/mvP
hqCZPhnSXd6J4GUCAwEAAaOCA+kwggPlMB8GA1UdIwQYMBaAFFFo/5CvAgd1PMzZ
ZWRiohK4WXI7MB0GA1UdDgQWBBTqYVKy/gpAgOUgijA3JKDqpmxqqjAjBgNVHREE
HDAaggwqLmdpdGh1Yi5jb22CCmdpdGh1Yi5jb20wDgYDVR0PAQH/BAQDAgWgMB0G
A1UdJQQWMBQGCCsGAQUFBwMBBggrBgEFBQcDAjB1BgNVHR8EbjBsMDSgMqAwhi5o
dHRwOi8vY3JsMy5kaWdpY2VydC5jb20vc2hhMi1oYS1zZXJ2ZXItZzUuY3JsMDSg
MqAwhi5odHRwOi8vY3JsNC5kaWdpY2VydC5jb20vc2hhMi1oYS1zZXJ2ZXItZzUu
Y3JsMEwGA1UdIARFMEMwNwYJYIZIAYb9bAEBMCowKAYIKwYBBQUHAgEWHGh0dHBz
Oi8vd3d3LmRpZ2ljZXJ0LmNvbS9DUFMwCAYGZ4EMAQICMIGDBggrBgEFBQcBAQR3
MHUwJAYIKwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmRpZ2ljZXJ0LmNvbTBNBggrBgEF
BQcwAoZBaHR0cDovL2NhY2VydHMuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0U0hBMkhp
Z2hBc3N1cmFuY2VTZXJ2ZXJDQS5jcnQwDAYDVR0TAQH/BAIwADCCAfQGCisGAQQB
1nkCBAIEggHkBIIB4AHeAHUApLkJkLQYWBSHuxOizGdwCjw1mAT5G9+443fNDsgN
3BAAAAFZspw+SwAABAMARjBEAiAerA7v0bVTR1blKT6IzEFdNAbS9J2+sMIyN6Da
d8QGQgIgA94eKijn12cROhrzND6+thVW/PdImUzTEodCGFgaCPUAdgBWFAaaL9fC
7NP14b1Esj7HRna5vJkRXMDvlJhV1onQ3QAAAVmynD8cAAAEAwBHMEUCICSmaZLA
KILGLXy9tbDCRcqKx4KaXaOFICxUHLDavhvTAiEAiiXvucr1ZYHcoJ1ix+/UAyW4
Sy1+SfIxV//PVuMumFcAdQDuS723dc5guuFCaR+r4Z5mow9+X7By2IMAxHuJeqj9
ywAAAVmynEAuAAAEAwBGMEQCIHKrzA5IbzNEGMr2NmbmcJncuUTZHsHRJqU0eCZc
ian5AiBGGIX5adWUbjuFWiBb1ZnEkYsH81/ozbYGm5xY36IkxwB2ALvZ37wfinG1
k5Qjl6qSe0c4V5UKq1LoGpCWZDaOHtGFAAABWbKcPl0AAAQDAEcwRQIhAJJMvQld
/c79AthFEj02qVdazf/Smjkb+ggN/DjrSB28AiAGEjSwxCj4r+PnUgzFtURbLofk
3iAvECLj9NZO2SZbdTANBgkqhkiG9w0BAQsFAAOCAQEAfIMvSUq9Z4EUniI976aO
kXTSPwa8GT+KFzlLpcyPmcU/x8ATptUsARnS96Yzx7BWtchprXsDWKdFLgmQ/YTT
dgUfy/Qyy7baJvCyLwBf4cJpsBdYaKxcige2dnAJjgVIvl8jEO4k+lD5BWgqQgRE
lDXj0SVVQQ1wd0MZTKWlDVbxmKsXzu5I0kWCG6/mfBcJc+6H+ABWc1YIK+pLP1jD
YcC8wj9fRkTCpZW/3lZ/Nt+snM1ujTRZ7RTBlRG2uJLpIX15JihSprEr3u39RHUp
HOODLNzVAw63zfJqCJvPtaCr+/KXKrqfjk9Z+e7Nmg+IxOf4M/Mxbox4KJvLlX8p
wQ==
-----END CERTIFICATE-----
",
// DigiCert High Assurance EV Root CA
@"-----BEGIN CERTIFICATE-----
MIIDxTCCAq2gAwIBAgIQAqxcJmoLQJuPC3nyrkYldzANBgkqhkiG9w0BAQUFADBs
MQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3
d3cuZGlnaWNlcnQuY29tMSswKQYDVQQDEyJEaWdpQ2VydCBIaWdoIEFzc3VyYW5j
ZSBFViBSb290IENBMB4XDTA2MTExMDAwMDAwMFoXDTMxMTExMDAwMDAwMFowbDEL
MAkGA1UEBhMCVVMxFTATBgNVBAoTDERpZ2lDZXJ0IEluYzEZMBcGA1UECxMQd3d3
LmRpZ2ljZXJ0LmNvbTErMCkGA1UEAxMiRGlnaUNlcnQgSGlnaCBBc3N1cmFuY2Ug
RVYgUm9vdCBDQTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAMbM5XPm
+9S75S0tMqbf5YE/yc0lSbZxKsPVlDRnogocsF9ppkCxxLeyj9CYpKlBWTrT3JTW
PNt0OKRKzE0lgvdKpVMSOO7zSW1xkX5jtqumX8OkhPhPYlG++MXs2ziS4wblCJEM
xChBVfvLWokVfnHoNb9Ncgk9vjo4UFt3MRuNs8ckRZqnrG0AFFoEt7oT61EKmEFB
Ik5lYYeBQVCmeVyJ3hlKV9Uu5l0cUyx+mM0aBhakaHPQNAQTXKFx01p8VdteZOE3
hzBWBOURtCmAEvF5OYiiAhF8J2a3iLd48soKqDirCmTCv2ZdlYTBoSUeh10aUAsg
EsxBu24LUTi4S8sCAwEAAaNjMGEwDgYDVR0PAQH/BAQDAgGGMA8GA1UdEwEB/wQF
MAMBAf8wHQYDVR0OBBYEFLE+w2kD+L9HAdSYJhoIAu9jZCvDMB8GA1UdIwQYMBaA
FLE+w2kD+L9HAdSYJhoIAu9jZCvDMA0GCSqGSIb3DQEBBQUAA4IBAQAcGgaX3Nec
nzyIZgYIVyHbIUf4KmeqvxgydkAQV8GK83rZEWWONfqe/EW1ntlMMUu4kehDLI6z
eM7b41N5cdblIZQB2lWHmiRk9opmzN6cN82oNLFpmyPInngiK3BD41VHMWEZ71jF
hS9OMPagMRYjyOfiZRYzy78aG6A9+MpeizGLYAiJLQwGXFK3xPkKmNEVX58Svnw2
Yzi9RKR/5CYrCsSXaQ3pjOLAEFe4yHYSkVXySGnYvCoCWw9E1CAx2/S6cCZdkGCe
vEsXCS+0yx5DaMkHJ8HSXPfqIbloEpw8nL+e/IBcm2PN7EeqJSdnoDfzAIJ9VNep
+OkuE6N36B9K
-----END CERTIFICATE-----
",
// DigiCert SHA2 Extended Validation Server CA
@"-----BEGIN CERTIFICATE-----
MIIEtjCCA56gAwIBAgIQDHmpRLCMEZUgkmFf4msdgzANBgkqhkiG9w0BAQsFADBs
MQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3
d3cuZGlnaWNlcnQuY29tMSswKQYDVQQDEyJEaWdpQ2VydCBIaWdoIEFzc3VyYW5j
ZSBFViBSb290IENBMB4XDTEzMTAyMjEyMDAwMFoXDTI4MTAyMjEyMDAwMFowdTEL
MAkGA1UEBhMCVVMxFTATBgNVBAoTDERpZ2lDZXJ0IEluYzEZMBcGA1UECxMQd3d3
LmRpZ2ljZXJ0LmNvbTE0MDIGA1UEAxMrRGlnaUNlcnQgU0hBMiBFeHRlbmRlZCBW
YWxpZGF0aW9uIFNlcnZlciBDQTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoC
ggEBANdTpARR+JmmFkhLZyeqk0nQOe0MsLAAh/FnKIaFjI5j2ryxQDji0/XspQUY
uD0+xZkXMuwYjPrxDKZkIYXLBxA0sFKIKx9om9KxjxKws9LniB8f7zh3VFNfgHk/
LhqqqB5LKw2rt2O5Nbd9FLxZS99RStKh4gzikIKHaq7q12TWmFXo/a8aUGxUvBHy
/Urynbt/DvTVvo4WiRJV2MBxNO723C3sxIclho3YIeSwTQyJ3DkmF93215SF2AQh
cJ1vb/9cuhnhRctWVyh+HA1BV6q3uCe7seT6Ku8hI3UarS2bhjWMnHe1c63YlC3k
8wyd7sFOYn4XwHGeLN7x+RAoGTMCAwEAAaOCAUkwggFFMBIGA1UdEwEB/wQIMAYB
Af8CAQAwDgYDVR0PAQH/BAQDAgGGMB0GA1UdJQQWMBQGCCsGAQUFBwMBBggrBgEF
BQcDAjA0BggrBgEFBQcBAQQoMCYwJAYIKwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmRp
Z2ljZXJ0LmNvbTBLBgNVHR8ERDBCMECgPqA8hjpodHRwOi8vY3JsNC5kaWdpY2Vy
dC5jb20vRGlnaUNlcnRIaWdoQXNzdXJhbmNlRVZSb290Q0EuY3JsMD0GA1UdIAQ2
MDQwMgYEVR0gADAqMCgGCCsGAQUFBwIBFhxodHRwczovL3d3dy5kaWdpY2VydC5j
b20vQ1BTMB0GA1UdDgQWBBQ901Cl1qCt7vNKYApl0yHU+PjWDzAfBgNVHSMEGDAW
gBSxPsNpA/i/RwHUmCYaCALvY2QrwzANBgkqhkiG9w0BAQsFAAOCAQEAnbbQkIbh
hgLtxaDwNBx0wY12zIYKqPBKikLWP8ipTa18CK3mtlC4ohpNiAexKSHc59rGPCHg
4xFJcKx6HQGkyhE6V6t9VypAdP3THYUYUN9XR3WhfVUgLkc3UHKMf4Ib0mKPLQNa
2sPIoc4sUqIAY+tzunHISScjl2SFnjgOrWNoPLpSgVh5oywM395t6zHyuqB8bPEs
1OG9d4Q3A84ytciagRpKkk47RpqF/oOi+Z6Mo8wNXrM9zwR4jxQUezKcxwCmXMS1
oVWNWlZopCJwqjyBcdmdqEU79OX2olHdx3ti6G8MdOu42vi/hw15UJGQmxg7kVkn
8TUoE6smftX3eg==
-----END CERTIFICATE-----
",
// Amazon S3 (provider for the release download) *.s3.amazonaws.com
@"-----BEGIN CERTIFICATE-----
MIIFWjCCBEKgAwIBAgIQBVG1kvpTzyBSuLcPJ1zBWTANBgkqhkiG9w0BAQsFADBk
MQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3
d3cuZGlnaWNlcnQuY29tMSMwIQYDVQQDExpEaWdpQ2VydCBCYWx0aW1vcmUgQ0Et
MiBHMjAeFw0xNzA5MjIwMDAwMDBaFw0xOTAxMDMxMjAwMDBaMGsxCzAJBgNVBAYT
AlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdTZWF0dGxlMRgwFgYD
VQQKEw9BbWF6b24uY29tIEluYy4xGzAZBgNVBAMMEiouczMuYW1hem9uYXdzLmNv
bTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKK5it42LH/8WFaEE7O9
4XJMg48TengCM25C9O0dUaPGOnZ1+oGf7DNvdo/MGVoAW5hfPX9WqjY30KyuKiFv
8uAbyX+9Bzq8KMLjdvGxzYMqbSxeAPSBf1yMdCyDEc8gW+vh2uXO+x9QePgiOAoO
qujLzyS/p7pSWPUKUH8kpMgP99RPoD/lRNbPAFr3QeJr7D/x3xNTKBHCbpE4SxBH
QM02GCu2DSYQtgm3hfZmD79BB1Ej4U1vM0hgiOu9eFLwmycbnrnU8sa4DkvmUJlv
xiIP5PvNweawm/QeoH6Qk/zDF30nr+5Av9IUhE4uAhl1H2OMqPp79SfJ2wxtvmNt
3z0CAwEAAaOCAf8wggH7MB8GA1UdIwQYMBaAFMASsih0aEZn6XAldBoARVsGfVxE
MB0GA1UdDgQWBBSQFZo7H1VA7uODvdTP1I6idMLqyDAvBgNVHREEKDAmghIqLnMz
LmFtYXpvbmF3cy5jb22CEHMzLmFtYXpvbmF3cy5jb20wDgYDVR0PAQH/BAQDAgWg
MB0GA1UdJQQWMBQGCCsGAQUFBwMBBggrBgEFBQcDAjCBgQYDVR0fBHoweDA6oDig
NoY0aHR0cDovL2NybDMuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0QmFsdGltb3JlQ0Et
MkcyLmNybDA6oDigNoY0aHR0cDovL2NybDQuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0
QmFsdGltb3JlQ0EtMkcyLmNybDBMBgNVHSAERTBDMDcGCWCGSAGG/WwBATAqMCgG
CCsGAQUFBwIBFhxodHRwczovL3d3dy5kaWdpY2VydC5jb20vQ1BTMAgGBmeBDAEC
AjB5BggrBgEFBQcBAQRtMGswJAYIKwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmRpZ2lj
ZXJ0LmNvbTBDBggrBgEFBQcwAoY3aHR0cDovL2NhY2VydHMuZGlnaWNlcnQuY29t
L0RpZ2lDZXJ0QmFsdGltb3JlQ0EtMkcyLmNydDAMBgNVHRMBAf8EAjAAMA0GCSqG
SIb3DQEBCwUAA4IBAQBydwnbmrwTTwhtCckZ9lDGoE+tdDFchjFG59dyzX1I4bHV
thVUwjEoc3VxngvmKb3cqLcHzXHNdoEdffRtw0IiEerPRsk7Nxh2p1HWR3cu/Hyp
kqGlObJADiw1DX4UDob735hevqcGN9M0rn9P/AbPK0HCn5uokLDmbeqe6u1YgdJL
m4skA/WsUsMCfsZY27RiZ+B162+laDD0oRiovOx5zKrzG/3yAFVutz2Bfud3QenU
3zt14ttnzFCcMlglW9oqffHMwQ1Smx/30ccDonoa1E02hnNmxeLyBwomPqd/ZYy6
zSJW0aSi1DadkZsifpr65AwgSuN5uGEhQas0glsN
-----END CERTIFICATE-----
"
        };


        public static IEnumerator LoadReleases(string url, string authToken = null)
        {
            WWW www = string.IsNullOrEmpty(authToken) ?
                new WWW (url) :
                new WWW (
                    url,
                    null,
                    new Dictionary<string, string>
                    {
                        { "Authorization", "token " + authToken }
                    });
            while (www.isDone == false)
                yield return null;
            yield return www;

            if(!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError(www.error);
                Debug.LogWarning(www.text);
            }
            else
            {
                yield return ParseReleases(www.text);
            }
        }

        private static GitHubRelease[] ParseReleases(string jsonString)
        {
            if (jsonString == null) return null;
            
            List<GitHubRelease> releaseList = new List<GitHubRelease>();

            System.Object obj = MiniJSON.Json.Deserialize (jsonString);
            List<System.Object> releases = (List<System.Object>)obj;

            foreach (Dictionary<string, object> releaseObject in releases)
            {
                GitHubRelease release = new GitHubRelease
                {
                    tag = (string)releaseObject["tag_name"],
                    body = (string)releaseObject["body"],
                    id = (long)releaseObject["id"]
                };
                release.assets = ExtractAssets(releaseObject);
                releaseList.Add(release);
            }

            releaseList.Sort((relA, relB) => relA.id.CompareTo(relB.id));
            return releaseList.ToArray();
        }

        private static GitHubAsset[] ExtractAssets(Dictionary<string, object> release)
        {
            if (release["assets"] == null) return null;

            List<GitHubAsset> assetList = new List<GitHubAsset>();

            List<System.Object> assets = (List<System.Object>)release["assets"];
            foreach (Dictionary<string, object> asset in assets) {
                assetList.Add(new GitHubAsset
                {
                    apiURL = (string)asset["url"],
                    htmlURL = (string)asset["browser_download_url"],
                    name = (string)asset["name"],
                    contentType = (string)asset["content_type"]
                });
            }

            return assetList.ToArray();
        }
        
        public static X509Certificate[] GetCertificates()
        {
            X509Certificate[] githubCertificates = new X509Certificate[githubCertificatesString.Length];
            for(int i = 0; i < githubCertificatesString.Length; i++)
            {
                githubCertificates[i] = new X509Certificate();
                githubCertificates[i].Import(Encoding.ASCII.GetBytes(githubCertificatesString[i]));
            }     

            return githubCertificates;
        }

        public static bool CertificateValidationCallback(
            System.Object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors
        )
        {
            bool correct = true;
            if(sslPolicyErrors == SslPolicyErrors.None) return true;       
            
            X509Certificate[] githubCertificates = GetCertificates();
            
            if(!(githubCertificates.Any(cert => cert.GetCertHashString() == certificate.GetCertHashString()) || UpliftPreferences.TrustUnknownCertificates()))
            {
                Debug.LogErrorFormat("The received certificate ({0}) is not known by Uplift. We cannot download the update package. You could update Uplift manually, or go to Preferences and set 'Trust unknown certificates' to true.", certificate.GetCertHashString());
                Debug.Log("Known certificates are:");
                foreach(X509Certificate cert in githubCertificates)
                    Debug.Log(" -- " + cert.GetCertHashString());
                return false;
            }

            for (int i=0; i < chain.ChainStatus.Length; i++) {
                if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown) {
                    continue;
                }
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan (0, 1, 0);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                bool chainIsValid = chain.Build ((X509Certificate2)certificate);
                if (!chainIsValid) {
                    correct = false;
                    break;
                }
            }

            return correct;
        }

        public static Stream GetAssetStream(GitHubAsset asset)
        {
            return GetAssetStream(asset, "");
        }
        public static Stream GetAssetStream(GitHubAsset asset, string token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(asset.apiURL);
            request.Method = "GET";
            if(!string.IsNullOrEmpty(token))
                request.Headers["Authorization"] = "token " + token;
            request.Accept = "application/octet-stream";
            request.UserAgent = "curl/7.47.0";
            request.AllowAutoRedirect = false;

            ServicePointManager.ServerCertificateValidationCallback = GitHub.CertificateValidationCallback;
            Uri address;
            HttpStatusCode responseStatus;
            using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                responseStatus = response.StatusCode;
                if((int)response.StatusCode < 300 || (int)response.StatusCode > 399)
                    throw new ApplicationException("Request should be redirected");
                address = new Uri(response.GetResponseHeader("Location"));
            }

            request = (HttpWebRequest)WebRequest.Create(address);
            request.Method = "GET";
            request.UserAgent = "curl/7.47.0";
            HttpWebResponse finalResponse = (HttpWebResponse)request.GetResponse();
            responseStatus = finalResponse.StatusCode;
            if((int)finalResponse.StatusCode >= 200 && (int)finalResponse.StatusCode <= 299)
            {
                return finalResponse.GetResponseStream();
            }

            throw new ApplicationException("Could not get asset at " + asset.apiURL);
        }
    }

    // Based on https://developer.github.com/v3/repos/releases/
    public class GitHubRelease
    {
        public string tag;
        public string body;
        public long id;
        public GitHubAsset[] assets;
    }

    public class GitHubAsset
    {
        public string htmlURL;
        public string apiURL;
        public string name;
        public string contentType;
    }
}
