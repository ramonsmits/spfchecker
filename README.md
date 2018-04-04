# Sender Policy Framewor (SPF) DNS Checker

Simple tool that traverses [Sender Policy Framewor (SPF)](https://en.wikipedia.org/wiki/Sender_Policy_Framework) DNS data and generates a tree like text report for verification.

## Execute spfchecker

Execute spfchecker for a single domain:

```cmd
> .\spftree.exe google.com
```

Execute spfchecker for multiple domains:

```cmd
> .\spftree.exe google.com microsoft.com
```


## Output

A tree like result is rendered, indenting the data for every SPF include found. Each block shows the number of includes.

```cmd
google.com
  v=spf1 include:_spf.google.com ~all
  _spf.google.com
    v=spf1 include:_netblocks.google.com include:_netblocks2.google.com include:_netblocks3.google.com ~all
    _netblocks.google.com
      v=spf1 ip4:64.233.160.0/19 ip4:66.102.0.0/20 ip4:66.249.80.0/20 ip4:72.14.192.0/18 ip4:74.125.0.0/16 ip4:108.177.8.0/21 ip4:173.194.0.0/16 ip4:209.85.128.0/17 ip4:216.58.192.0/19 ip4:216.239.32.0/19 ~all
      count: 0
    _netblocks2.google.com
      v=spf1 ip6:2001:4860:4000::/36 ip6:2404:6800:4000::/36 ip6:2607:f8b0:4000::/36 ip6:2800:3f0:4000::/36 ip6:2a00:1450:4000::/36 ip6:2c0f:fb50:4000::/36 ~all
      count: 0
    _netblocks3.google.com
      v=spf1 ip4:172.217.0.0/19 ip4:172.217.32.0/20 ip4:172.217.128.0/19 ip4:172.217.160.0/20 ip4:172.217.192.0/19 ip4:108.177.96.0/19 ~all
      count: 0
    count: 3
  count: 4
```

The result ends with combining all SPF data into a single SPF record for each domain. This can potentially be used to simplify the SPF data. For example, the SPF specification has a DNS lookup limit of 10. If your domain is used on a lot of services then it can easily happen that your SPF configuration exceeds this limit. This can be circumvented by combining the SPF data this lookup limit. **Please note that this doesn't work for services that have an elastic SMTP server farm and SPF data continiously is updated.** In such case it probably is better to rely on [DKIM](https://en.wikipedia.org/wiki/DomainKeys_Identified_Mail) instead.

```
v=spf1 ip4:64.233.160.0/19 ip4:66.102.0.0/20 ip4:66.249.80.0/20 ip4:72.14.192.0/18 ip4:74.125.0.0/16 ip4:108.177.8.0/21 ip4:173.194.0.0/16 ip4:209.85.128.0/17 ip4:216.58.192.0/19 ip4:216.239.32.0/19 ip6:2001:4860:4000::/36 ip6:2404:6800:4000::/36 ip6:2607:f8b0:4000::/36 ip6:2800:3f0:4000::/36 ip6:2a00:1450:4000::/36 ip6:2c0f:fb50:4000::/36 ip4:172.217.0.0/19 ip4:172.217.32.0/20 ip4:172.217.128.0/19 ip4:172.217.160.0/20 ip4:172.217.192.0/19 ip4:108.177.96.0/19 ~all
```
