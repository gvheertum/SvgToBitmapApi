## SVG as service
Testing capabilities of the SVG component to be exposed as a service to be called by other tools. This should allow us to scale/balance in the future.

Tried 2 options:
- Azure Function
- WebApi

The function approach failed since the azure functions do not allow calls to the system.drawing namespaces. The webapi however seemed to be a workable option.

This library is a base version (very much work in progress) for a api implementation of the SVG component.
