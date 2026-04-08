# Original request

## Current user request

The bundle was reopened with this direction:

> I agree with your recomendation. Use [$candoitall-bundle-workflow](C:\\Users\\lucys\\.codex\\skills\\candoitall-bundle-workflow\\SKILL.md) to solve those points.
> 1) start with detailed refactoring first. Analyze whole solution and find the gaps in architecture. focus on very long files, sources of truth (canonical refactor), isolation of helpers, etc.
> 2) then implement the points what you described in your last response.
> 3) consider also this. I had it prepared as separated prompt, but it plays huge role in deeper context of project and it might solve your suggestions in comparison to sharptools:

## Embedded follow-up prompt

> Use [$candoitall-bundle-workflow](C:\\Users\\lucys\\.codex\\skills\\candoitall-bundle-workflow\\SKILL.md) to prepare and execute and validate bundle that will add this feature in our code analysis:
>
> Main Goal:
> - providing agent context that is preciselly focused to solving some trouble
> - save context window for agent and speed up development
>
> Why we are doing it:
> - when agent see some exception it usually loads whole files and analyze them. it can take lots of unnecessary context even the agent could use just few functions/classes/enums, etc from that file.
> - real programmers usually flow in code through relations between functions/classes, etc rather than reading always whole files. It would overwhelm them if they must read always whole files. It is same with LLMs. Reducing context to usefull information will help llm to focus on main trouble path. It is not universal rule, because sometimes it will need more information, but it can ask them via this system, rather than loading whole files all over again.
>
> How we can do it:
> - When agent find some exception or compile error it will be able to ask for related tree around it.
> - It will start from the function where exception/bug is. Then it will run recursive function to map all related into some asked depth. It means that it start mapping classes and functions that are in that broken function, from them it can go another steps in tree and map also their related functions and classes, etc. It must have some stop limits definitions, because some classes can be very large and we are trying to get just enough content around trouble to solve it.
>
> Notes:
> - C# has great reflections. We should be able to do it well.
> - Output for agent must contains also references to exact files so agent can decide to read whole file if wants or ask for deeper context around another related parts from that file.
> - optionally it could add also names+summary xmls if exists for whole specific file, so agent can just check names of functions and then ask for some details around some functions.
> - usually programmers remember the basic helpers, enums and classes. it might be interesting to find the way how to identify them (for example those that has major use across the project). then agent can keep them in kind of "ad hoc skill" that will be created during specific run only as temporary skill and it can always get back to it without analyzing it again (for example after compression of context, etc).

## Prior closure notes that triggered reopening

- The ER diagram became useful after relationship work.
- The whole-solution class diagram remained too noisy to be a strong orientation artifact.
- The persistence relationship collector still under-reported real schema relations.
- The current snapshot did not yet provide focused trouble-path context comparable to the way developers navigate code with SharpTools.
