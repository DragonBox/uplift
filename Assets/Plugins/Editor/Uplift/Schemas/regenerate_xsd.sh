#!/bin/bash

# Note: all files has to be processed at the same time,
#       or the generator will make duplicate classes, and you don't want that
Xsd *.xsd /classes /namespace:Uplift.Schemas
