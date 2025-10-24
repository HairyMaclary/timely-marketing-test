# Architecture and Delivery

1. *Part of the requirements for this site are that we are able to gather usage and marketing campaign metrics from visitors to the site. Describe how you would ensure our SEO and front-end performance could be optimised so as to not impact customers' experience of navigating the site while still allowing us to implement various front-end metrics.*

A quick search online reveals several strategies that we can use with Umbraco:

- Enable output caching via Umbraco’s OutputCache.
- Minify and bundle CSS/JS using tools like Smidge (integrated with Umbraco).
- Use image optimization (e.g., ImageProcessor or TinyPNG API) and lazy-load images.

Coming from a React perspective I would consider some options such server side rendering, lazy loading via React Router, minimising data size and metric requests. For example, we could consider only sending metrics when the data has changed.

And if performance is really important then we need to break into the dev tools and start making some performance measurements and see which functions are causing delays and refactor accordingly. I have seen that sometimes it is possible to load expensive set operations in parallel. However this involves careful global state management.

It is not likely to be as helpful, but it is also possible to make measurements such as TTI (Time to Interactive) via e2e tests.


2. *The Timely product is served to customers all over the world. Describe some of the tools and mechanisms you'd use to deliver content reliably across multiple geographic regions.*

I would look into the following:
 - multi-region hosting
 - language variations; while looking into this project I did see an option in the back office to "Allow editors to create content of different languages."
 - Searching online I see that Umbraco’s Content Delivery API allows you to serve content in JSON format to any frontend or channel. I am guessing that this allows you to talk to FEs built with different tech, such as NextJS.
 - run synthetic tests and health checks across all deployed regions at regular intervals.
 - have alerts (eg New Relic alerts) attached to those tests

 3. *Previously the team has relied on manually copying the compiled site across from a development environment to a staging or production environment. Discuss in detail how you'd ensure the deployment process was reliable and efficient.*

 We would want to introduce some kind of CI-CD pipeline. I've seen both TeamCity and Github actions being used to deploy build artifacts to a CDN. 

 Github actions can be configured as code. I have some breif experience doing this with Kotlin files. But I've also manually configured build steps in Team City.

 Usually the steps will involve linting, compiling/transpiling code, running automated tests, deploying to staging, running e2e tests over that environment, only then deploying to prod, and running the e2e tests against prod as well. 

 This will involve the set up of test organisations and the storage of sensitive data such as keys somewhere in that pipeline.

 4. *The company wants to run a campaign offering an extremely generous pricing offer for new and existing customers. How would you prepare for a large increase in customer traffic both before and during that campaign?*

 It really depends on the infrastructure you are working on. At my present place of employment we use Kubernetes, and set a threshold such as CPU usage for spinning up additional pods. We have alerts that fire if the pods are getting too stressed. 

 I've also seen lambda functions being used quite a bit. AWS will just keep adding more if the traffic gets busy, but they suffer from cold starts and really are not for full applications, but more just a specific function.

 Overall, this would be a topic which would require us to consult with an infrastructure expert.
