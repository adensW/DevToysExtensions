using Adens.DevToys.OpenApiToCode.Helper;
using SharpYaml.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Adens.DevToys.OpenApiToCode.Tests;
public class OpenApiCodeGeneratorHelperTests:TestBase
{

    [Fact]
    public async Task OpenApiCodeGeneratorShouldWork()
    {
        string filter = "(?i)/api/tagging-admin/entity-tags.*";
        string basePath = "C:/Users/A/Repos/Woo/Ego/npm/vue-packs/layers/tagging-management/server";
        List<CodeApiPathHandler> handlers = new List<CodeApiPathHandler>() { 
            new CodeApiPathHandler
            {
                OperationMatchRegex="(?i)get",
                PathMatchRegex=".*",
                PathTemplate=basePath+"""{{path|string.replace "{" "["|string.replace "}" "]"}}{{if path|string.ends_with "}" }}{{else}}/index{{end}}.{{operation | string.downcase }}.ts""",
                CodeTemplate=@"
import { getQuery } from 'h3'

import { createError } from '#imports'
import { getServerSession } from '#auth'
import type { Tag } from '@atiny/tagging'

export default defineEventHandler(async (event) => {
        const ssrHeader = new Headers()
  const session = await getServerSession(event)
  if (session === null || session.accessToken === null) {
    throw createError({ statusMessage: 'Unauthenticated', statusCode: 401 })
  }
ssrHeader.append('Authorization', `Bearer ${session!.accessToken}`)
  // const methosd = event.method
  // let body
  // if (method !== 'GET') body = await readRawBody(event)
  const params = getQuery(event)
  try {
    const res = await $fetch<Array<Tag>>('{{path}}', {
      method: 'GET',
      baseURL: import.meta.env.NUXT_TAGGING_BASE_URL,
      headers: ssrHeader,
      params: params,
    })
    return res
  }
  catch {
    throw createError({ statusMessage: 'Unauthenticated', statusCode: 401 })
    return []
  }
})
"
            },
            new CodeApiPathHandler
            {
                OperationMatchRegex="(?i)put",
                PathMatchRegex=".*",
               PathTemplate=basePath+"""{{path|string.replace "{" "["|string.replace "}" "]"}}{{if path|string.ends_with "}" }}{{else}}/index{{end}}.{{operation | string.downcase }}.ts""",
               CodeTemplate ="""
import { readRawBody } from 'h3'
import { createError } from '#imports'
import { getServerSession } from '#auth'
import type { Tag } from '@atiny/tagging'

export default defineEventHandler(async (event) => {
  const ssrHeader = new Headers()
  const session = await getServerSession(event)
  if (session === null || session.accessToken === null) {
    throw createError({ statusMessage: 'Unauthenticated', statusCode: 401 })
  }
  ssrHeader.append('Authorization', `Bearer ${session!.accessToken}`)
  // const methosd = event.method
  // let body
  const body = await readRawBody(event)
  const params = getQuery(event)
  const id = getRouterParam(event, 'id')
  try {
    const res = await $fetch<Tag>(`{{path|string.replace "{" "${"}}`, {
      method: 'PUT',
      baseURL: import.meta.env.NUXT_TAGGING_BASE_URL,
      headers: ssrHeader,
      body: body,
      params: params,
    })
    return res
  }
  catch {
    throw createError({ statusMessage: 'Unauthenticated', statusCode: 401 })
    return []
  }
})
"""
            },
            new CodeApiPathHandler
            {
                OperationMatchRegex="(?i)delete",
                PathMatchRegex=".*",
               PathTemplate=basePath + """{{path|string.replace "{" "["|string.replace "}" "]"}}{{if path|string.ends_with "}" }}{{else}}/index{{end}}.{{operation | string.downcase }}.ts""",
               CodeTemplate ="""
import { readRawBody } from 'h3'
import { createError } from '#imports'
import { getServerSession } from '#auth'
import type { Tag } from '@atiny/tagging'

export default defineEventHandler(async (event) => {
  const ssrHeader = new Headers()
  const session = await getServerSession(event)
  if (session === null || session.accessToken === null) {
    throw createError({ statusMessage: 'Unauthenticated', statusCode: 401 })
  }
  ssrHeader.append('Authorization', `Bearer ${session!.accessToken}`)
  // const methosd = event.method
  // let body
  const body = await readRawBody(event)
  const params = getQuery(event)
  const id = getRouterParam(event, 'id')
  try {
    const res = await $fetch<Tag>(`{{path|string.replace "{" "${"}}`, {
      method: 'DELETE',
      baseURL: import.meta.env.NUXT_TAGGING_BASE_URL,
      headers: ssrHeader,
      body: body,
      params: params,
    })
    return res
  }
  catch {
    throw createError({ statusMessage: 'Unauthenticated', statusCode: 401 })
    return []
  }
})
"""
            },
            new CodeApiPathHandler  
            {
                OperationMatchRegex="(?i)post",
                PathMatchRegex=".*",
               PathTemplate=basePath+"""{{path|string.replace "{" "["|string.replace "}" "]"}}{{if path|string.ends_with "}" }}{{else}}/index{{end}}.{{operation | string.downcase }}.ts""",
               CodeTemplate ="""
import { readRawBody } from 'h3'
import { createError } from '#imports'
import { getServerSession } from '#auth'
import type { Tag } from '@atiny/tagging'

export default defineEventHandler(async (event) => {
  const ssrHeader = new Headers()
  const session = await getServerSession(event)
  if (session === null || session.accessToken === null) {
    throw createError({ statusMessage: 'Unauthenticated', statusCode: 401 })
  }
  ssrHeader.append('Authorization', `Bearer ${session!.accessToken}`)
  // const methosd = event.method
  // let body
  const body = await readRawBody(event)
  try {
    const res = await $fetch<Tag>(`{{path|string.replace "{" "${"}}`, {
      method: 'POST',
      baseURL: import.meta.env.NUXT_TAGGING_BASE_URL,
      headers: ssrHeader,
      body: body,
    })
    return res
  }
  catch {
    throw createError({ statusMessage: 'Unauthenticated', statusCode: 401 })
    return []
  }
})
"""
            }
        };
        try
        {
            await OpenApiCodeGeneratorHelper.GenerateCode("https://localhost:44359/swagger/v1/swagger.json",filter, handlers);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
