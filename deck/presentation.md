---
marp: true
theme: gaia
class:
  - lead
  - invert
paginate: true
backgroundColor: #fff
backgroundImage: url('./images/background_1.jpg')
---

# Observability of Distributed .NET Applications

Erik Svensson, Webstep, 2020

---
<!-- backgroundImage: url('./images/background_2.jpg') -->

# The Three Pillars of Observability

_Increasingly important as we move to distributed_

- __Tracing__ - Follow the request flow
- __Metrics__ - Measure over time
- __Logging__ - Record key events

---

<!-- backgroundImage: url('./images/background_2.jpg') -->

# The State of Observability Tooling

_A lot has happened in 2020_

- __Vendored Solutions / Lacking Conventions__
- __OpenTracing + OpenCensus = OpenTelemetry__
- __Observability as a First Class Citizen__

---

<!-- backgroundImage: url('./images/background_2.jpg') -->

# OpenTelemetry Overview

```text

    +---------+   host | 
    | Service |        |
    +----+----+        |
         |             |
         v             |
   +-----+-----+       |       +------------------+       +------------+
   | Collector +-------+------>+ Exporter Backend +<------+ Monitoring |
   +-----------+       |       +------------------+       +------------+

```
---

<!-- backgroundImage: url('./images/background_1.jpg') -->

# OpenTelemetry and .NET

_Almost there but not quite..._

- __Tracing__: 🙂
- __Metrics__: 😐
- __Logging__: 🙁

---
<!-- backgroundImage: url('./images/background_1.jpg') -->

# Today's Excercise

_Add Observability to a Microservice System_

- __Small effort that will bring lots of value__
- __Simulate production environment with Docker__
- __Visualize in Grafana__

---

# Step 1: Add Tracing

```text
                                 Trace Span

            start                                           stop
            |    |---------------GetCustomer---------------|
            |      |---Authenticate---||---CallDatabase---|
            |        |---GetUser---|
            |         |--Http GET--|
            |           |--GetUser--|
            v     
            nesting ---------------------------------------> time
```