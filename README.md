# logtube
[![Build and Test](https://github.com/Marcin99b/logtube/actions/workflows/build.yml/badge.svg)](https://github.com/Marcin99b/logtube/actions/workflows/build.yml)

Search and analysis for logs written in Rust 🦀

# Roadmap

#### First stage

- [x] Store structured logs on disc
- [x] Expose tcp endpoint for logs gathering
- [ ] Select tool for storing logs
  - https://github.com/gree/flare is used by Seq
- [ ] Basic search
  - [ ] Return logs from that contain sentence
  - [ ] Return logs from time period
  - [ ] Count logs quantity per selected time range
- [x] Serilog (C#) integration
- [ ] Use api keys in requests
- [ ] Own log structure format
- [ ] Configuration file
